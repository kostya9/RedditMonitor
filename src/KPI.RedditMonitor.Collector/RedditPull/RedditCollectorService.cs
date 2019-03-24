using System;
using System.Threading;
using System.Threading.Tasks;
using KPI.RedditMonitor.Collector.RedditPull.Collectors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KPI.RedditMonitor.Collector.RedditPull
{
    public class RedditCollectorService : BackgroundService
    {
        private readonly IRedditCollector _collector;
        private readonly PostInserter _inserter;
        private readonly ILogger<RedditCollectorService> _log;

        public RedditCollectorService(PostInserter inserter, IRedditCollector collector,
            ILogger<RedditCollectorService> log)
        {
            _inserter = inserter;
            _collector = collector;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log.LogInformation("Started collecting reddit entries...");

            var inserterTask = _inserter.Run(stoppingToken);

            var count = 0;
            var lastTime = DateTime.UtcNow;

            await _collector.SubscribeOnEntries(e =>
            {
                var imagePosts = ImagePostFactory.Create(e.Id, e.Text, e.Url, e.CreatedAt, e.Ignore);

                foreach (var imagePost in imagePosts) _inserter.Add(imagePost);

                if (++count % 1000 == 0)
                {
                    var deltaSeconds = (DateTime.UtcNow - lastTime).TotalSeconds;
                    _log.LogInformation($"Received 1000 posts in {deltaSeconds}s");
                    lastTime = DateTime.UtcNow;
                }
            }, stoppingToken);

            await inserterTask;

            _log.LogInformation("Stopping collecting reddit entries...");
        }
    }
}