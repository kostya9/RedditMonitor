using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using KPI.RedditMonitor.Application.Similarity;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.Memory;
using Microsoft.Extensions.Logging.Abstractions;

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

            _repository = new ImagePostsRepository(mongoClient, NullLogger<ImagePostsRepository>.Instance);
            _httpClient = new HttpClient();
            // Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();
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
            var maxImageSize = 3 * 1024 * 1024;
            var inserted = 0;

            foreach (var record in evnt.Records)
            {
                var imagePost = JsonConvert.DeserializeObject<ImagePost>(record.Body);
                using (var response = await _httpClient.GetAsync(imagePost.ImageUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    if(!response.IsSuccessStatusCode)
                    {
                        context.Logger.LogLine($"StatusCode {response.StatusCode} was not successful while doing request to {imagePost.ImageUrl}, so skipping image");
                        continue;
                    }

                    if (response.Content.Headers.TryGetValues("Content-Length", out var values) 
                        && values.Any((v) => int.Parse(v) > maxImageSize))
                    {
                        context.Logger.LogLine($"Could not save image {imagePost.ImageUrl} - it was too big");
                        continue;
                    }

                    using (var content = await response.Content.ReadAsStreamAsync())
                    {
                        var features = ImageFeatureFactory.Create(content);
                        
                        imagePost.FeatureBuckets = features.GetBuckets();
                    }
                }

                var duplicateError = "E11000 duplicate key error index";
                try
                {
                    await _repository.Add(imagePost);
                    inserted++;
                }
                catch (MongoWriteException e)
                    when (e.Message.Contains(duplicateError))
                {
                    context.Logger.LogLine($"Could not save {imagePost.ImageUrl}, seems that it was already inserted");
                }

            }

            context.Logger.LogLine($"Inserted {inserted} posts");
        }
    }
}
