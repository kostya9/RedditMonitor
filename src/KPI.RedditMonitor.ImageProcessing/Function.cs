using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
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

            await _repository.AddRange(posts);

            context.Logger.LogLine($"Inserted {posts.Length} posts");
        }
    }
}
