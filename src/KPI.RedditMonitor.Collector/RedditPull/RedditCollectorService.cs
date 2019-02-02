using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KPI.RedditMonitor.Collector.RedditPull
{
    public class RedditCollectorService : BackgroundService
    {
        private readonly PostInserter _inserter;
        private readonly RedditCollector _collector;
        private readonly ILogger<RedditCollectorService> _log;

        public RedditCollectorService(PostInserter inserter, RedditCollector collector, ILogger<RedditCollectorService> log)
        {
            _inserter = inserter;
            _collector = collector;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log.LogInformation("Started collecting reddit entries...");

            _inserter.Start();

            var count = 0;
            var lastTime = DateTime.UtcNow;

            await _collector.SubscribeOnEntries((e) =>
            {
                var imagePosts = ImagePostFactory.Create(e.Id, e.Text, e.Url, e.CreatedAt, e.Ignore);

                foreach (var imagePost in imagePosts)
                {
                    _inserter.Add(imagePost);
                }

                if (++count % 1000 == 0)
                {
                    var deltaSeconds = (DateTime.UtcNow - lastTime).TotalSeconds;
                    _log.LogInformation($"Received 1000 posts in {deltaSeconds}s");
                    lastTime = DateTime.UtcNow;
                }
            }, stoppingToken);

            _inserter.Stop();

            _log.LogInformation("Stopping collecting reddit entries...");
        }
    }
}
