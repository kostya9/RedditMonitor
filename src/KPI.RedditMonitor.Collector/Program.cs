using System;
using System.Threading.Tasks;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MongoDB.Driver;
using RedditSharp;
using RedditSharp.Things;

namespace KPI.RedditMonitor.Collector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var connectionString = config["MongoDb:ConnectionString"];
            var repo = new ImagePostsRepository(new MongoClient(connectionString));

            var redditOptions = config.GetSection("Reddit").Get<RedditOptions>();
            var collector = new RedditCollector(redditOptions);

            var builder = new LoggerFactory().AddConsole();

            var log = builder.CreateLogger<Program>();
            var inserter = new PostInserter(repo, builder.CreateLogger<PostInserter>(), 100);

            await Run(inserter, collector, log);
        }

        private static async Task Run(PostInserter inserter, RedditCollector collector, ILogger<Program> log)
        {
            inserter.Start();

            var count = 0;
            var lastTime = DateTime.UtcNow;

            await collector.SubscribeOnEntries((e) =>
            {
                var imagePosts = ImagePostFactory.Create(e.Id, e.Text, e.Url);

                foreach (var imagePost in imagePosts)
                {
                    inserter.Add(imagePost);
                }

                if (++count % 1000 == 0)
                {
                    var deltaSeconds = (DateTime.UtcNow - lastTime).TotalSeconds;
                    log.LogInformation($"Received 1000 posts in {deltaSeconds}s");
                    lastTime = DateTime.UtcNow;
                }
            });

            inserter.Stop();
        }
    }
}
