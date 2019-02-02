using System;
using KPI.RedditMonitor.Collector.RedditPull;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace KPI.RedditMonitor.Collector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((builder) =>
                {
                    builder
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.Development.json", optional: true)
                        .AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<MongoDbConfig>(c => context.Configuration.Bind("MongoDb", c));
                    services.Configure<RedditOptions>(o => context.Configuration.Bind("Reddit", o));

                    services.AddSingleton<IMongoClient>(p =>
                    {
                        var config = p.GetRequiredService<IOptions<MongoDbConfig>>();
                        return new MongoClient(config.Value.ConnectionString);
                    });

                    services.AddSingleton<ImagePostsRepository>();
                    services.AddSingleton<RedditCollector>();
                    services.AddSingleton<PostInserter>(p =>
                    {
                        var log = p.GetRequiredService<ILogger<PostInserter>>();
                        var repo = p.GetRequiredService<ImagePostsRepository>();
                        return new PostInserter(repo, log, 30);
                    });

                    services.AddHostedService<RedditCollectorService>();
                })
                .Build();

            host.Run();
        }
    }
}
