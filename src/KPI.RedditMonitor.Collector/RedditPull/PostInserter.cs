using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Logging;

namespace KPI.RedditMonitor.Collector.RedditPull
{
    public class PostInserter
    {
        private readonly int _delaySeconds;
        private readonly ILogger<PostInserter> _log;
        private readonly ConcurrentQueue<ImagePost> _posts;
        private readonly ImagePostsRepository _repository;

        public PostInserter(ImagePostsRepository repository, ILogger<PostInserter> log, int delaySeconds)
        {
            _repository = repository;
            _log = log;
            _delaySeconds = delaySeconds;
            _posts = new ConcurrentQueue<ImagePost>();
        }

        public void Add(ImagePost post)
        {
            _posts.Enqueue(post);
        }

        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var insertTask = Task.CompletedTask;
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_delaySeconds), cancellationToken);
                        try
                        {
                            await insertTask;

                            var toInsert = new List<ImagePost>();
                            while (_posts.TryDequeue(out var post)) toInsert.Add(post);

                            if (toInsert.Any())
                            {
                                insertTask = _repository.AddRange(toInsert);
                                _log.LogInformation(
                                    $"[STATS]: Received {toInsert.Count} images with posts in {_delaySeconds} seconds");
                            }
                        }
                        catch (Exception e)
                        {
                            _log.LogError(e, "An error occurred during post inserting");
                            throw;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _log.LogInformation("Inserter task was cancelled");
                    }
                }
            });
        }
    }
}