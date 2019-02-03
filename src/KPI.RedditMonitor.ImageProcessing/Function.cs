using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using KPI.RedditMonitor.Data;
using KPI.RedditMonitor.ImageProcessing.Similarity;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.Memory;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace KPI.RedditMonitor.ImageProcessing
{
    public class Function
    {
        private readonly ImagePostsRepository _repository;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.Development.json", true)
                .AddEnvironmentVariables()
                .Build();
            var mongoClient = new MongoClient(config["MongoDb:ConnectionString"]);

            _repository = new ImagePostsRepository(mongoClient);
            _httpClient = new HttpClient();
            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();
        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            var maxImageSize = 5 * 1024 * 1024;
            var inserted = 0;
            var posts = evnt.Records.Select(e => JsonConvert.DeserializeObject<ImagePost>(e.Body));

            foreach (var imagePost in posts)
            {
                using (var response =
                    await _httpClient.GetAsync(imagePost.ImageUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    int length = int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value
                        .First());

                    if (length > maxImageSize)
                    {
                        context.Logger.LogLine($"Could not save image {imagePost.ImageUrl} - it was too big");
                        continue;
                    }

                    using (var content = await response.Content.ReadAsStreamAsync())
                    {
                        var features = GetFeatures(imagePost.ImageUrl, content);

                        imagePost.FeatureBuckets = new Dictionary<string, double[]>();
                        foreach (var featuresHistogram in features.Histograms)
                        {
                            imagePost.FeatureBuckets[featuresHistogram.Name] = featuresHistogram.GetBuckets();
                        }
                    }
                }

                try
                {
                    await _repository.Add(imagePost);
                    inserted++;
                }
                catch (AggregateException aggregate) when (aggregate.InnerExceptions.Any(e => e.Message.Contains("E11000 duplicate key error index")))
                {
                    context.Logger.LogLine($"Could not save {imagePost.ImageUrl}, seems that it was already inserted");
                }
                
            }

            context.Logger.LogLine($"Inserted {inserted} posts");
        }

        private ImageFeatures GetFeatures(string url, Stream content)
        {
            var histograms = new ImageFeatures(url);
            using (var image = Image.Load(content))
            {
                var buckets = 4;
                var minRgb = 0;
                var maxRgb = 255;
                var red = histograms.Add("red", buckets, minRgb, maxRgb);
                var green = histograms.Add("green", buckets, minRgb, maxRgb);
                var blue = histograms.Add("blue", buckets, minRgb, maxRgb);

                foreach (var pixel in image.GetPixelSpan())
                {
                    red.Add(pixel.R);
                    blue.Add(pixel.B);
                    green.Add(pixel.G);
                }
            }

            return histograms;
        }
    }
}
