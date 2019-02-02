using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KPI.RedditMonitor.Collector.RedditPull
{
    public class PostInserter
    {
        private readonly ILogger<PostInserter> _log;
        private readonly ConcurrentQueue<ImagePost> _posts;
        private readonly IAmazonSQS _sqs;
        private readonly IOptions<PostQueueOptions> _options;
        private readonly ImagePostsRepository _repository;

        public PostInserter(IAmazonSQS sqs, IOptions<PostQueueOptions> options, ImagePostsRepository repository, ILogger<PostInserter> log)
        {
            _sqs = sqs;
            _options = options;
            _repository = repository;
            _log = log;
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
                        await Task.Delay(TimeSpan.FromSeconds(_options.Value.BatchDelay), cancellationToken);
                        try
                        {
                            await insertTask;
                            
                            _log.LogInformation(
                                $"[STATS]: Received {_posts.Count} images with posts in {_options.Value.BatchDelay} seconds");
                            await Task.WhenAll(BatchPosts().Select(b => SendMessages(b, cancellationToken)));
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

        private IEnumerable<IEnumerable<ImagePost>> BatchPosts()
        {
            var batchSize = 10;
            while (_posts.Any())
            {
                var batch = new List<ImagePost>(10);
                for (int i = 0; i < batchSize && _posts.TryDequeue(out var post); i++)
                {
                    batch.Add(post);
                }

                yield return batch;
            }
        }

        private async Task SendMessages(IEnumerable<ImagePost> posts, CancellationToken cancellationToken)
        {
            await _sqs.SendMessageBatchAsync(new SendMessageBatchRequest
            {
                QueueUrl = _options.Value.QueueUrl,
                Entries = posts.Select(p => new SendMessageBatchRequestEntry
                {
                    Id = Guid.NewGuid().ToString("D"),
                    MessageBody = JsonConvert.SerializeObject(p)
                }).ToList()
            }, cancellationToken);
        }
    }
}