using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Logging;

namespace KPI.RedditMonitor.Collector
{
    public class PostInserter
    {
        private readonly ImagePostsRepository _repository;
        private readonly ILogger<PostInserter> _log;
        private readonly int _delaySeconds;
        private readonly ConcurrentQueue<ImagePost> _posts;

        private bool _running;

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

        public void Start()
        {
            _running = true;
            Task.Run(async () =>
            {
                var insertTask = Task.CompletedTask;
                try
                {
                    while (_running)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_delaySeconds));
                        await insertTask;

                        var toInsert = new List<ImagePost>();
                        while (_posts.TryDequeue(out var post))
                        {
                            toInsert.Add(post);
                        }

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
            });
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
