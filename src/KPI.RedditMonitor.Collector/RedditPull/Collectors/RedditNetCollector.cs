﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Collector.RedditPull.Collectors
{
    public class RedditNetCollector : IRedditCollector
    {
        private readonly ILogger<RedditNetCollector> _log;
        private readonly RedditOptions _options;

        public RedditNetCollector(IOptions<RedditOptions> options, ILogger<RedditNetCollector> log)
        {
            _options = options.Value;
            _log = log;
        }
        public Task SubscribeOnEntries(Action<RedditPost> callback, CancellationToken cancellationToken)
        {
            var api = new RedditAPI(_options.ClientId, _options.RefreshToken, _options.ClientSecret);

            var all = api.Subreddit("all");
            all.Posts.NewUpdated += (s, args) =>
            {
                foreach(var added in args.Added)
                {
                    var t = added.Listing;
                    callback(new RedditPost(t.Id, t.Title + " " + t.SelfText + " " + t.URL,
                        t.Permalink.ToString(), t.CreatedUTC, added.NSFW));
                }
            };
            all.Posts.MonitorNew();

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => taskCompletionSource.TrySetResult(true));
            return taskCompletionSource.Task;
        }
    }
}