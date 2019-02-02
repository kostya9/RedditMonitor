using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace KPI.RedditMonitor.ImageProcessing
{
    public class Function
    {
        private readonly ImagePostsRepository _repository;
        private readonly AmazonS3Client _s3;
        private readonly HttpClient _httpClient;
        private readonly string _bucket;

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

            _bucket = "euw-1-redditmonitor-images";
            _repository = new ImagePostsRepository(mongoClient);
            _s3 = new AmazonS3Client();
            _httpClient = new HttpClient();
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
            var posts = evnt.Records.Select(e => JsonConvert.DeserializeObject<ImagePost>(e.Body)).ToArray();

            foreach (var imagePost in posts)
            {
                var response = await _httpClient.GetAsync(imagePost.ImageUrl);
                var stream = await response.Content.ReadAsStreamAsync();
                var fileName = imagePost.ImageUrl.Split("/").Last();


                var imageName = $"{Guid.NewGuid():N}/{fileName}";
                await _s3.PutObjectAsync(new PutObjectRequest()
                {
                    InputStream = stream,
                    Key = imageName,
                    AutoCloseStream = true,
                    CannedACL = S3CannedACL.PublicRead,
                    BucketName = _bucket
                });
                imagePost.S3Path = imageName;
                await _repository.Add(imagePost);
            }

            context.Logger.LogLine($"Inserted {posts.Length} posts");
        }
    }
}
