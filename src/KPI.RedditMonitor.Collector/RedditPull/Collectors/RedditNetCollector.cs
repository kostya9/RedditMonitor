using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers.EventArgs;
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
            EventHandler<PostsUpdateEventArgs> newUpdatedHandler = (s, args) =>
            {
                foreach(var added in args.Added)
                {
                    var t = added.Listing;
                    callback(new RedditPost(t.Id, t.Title + " " + t.SelfText + " " + t.URL,
                        t.Permalink.ToString(), t.CreatedUTC, added.NSFW, added.Subreddit));
                }
            };
            all.Posts.NewUpdated += newUpdatedHandler;
            all.Posts.MonitorNew();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => 
            {
                all.Posts.MonitorNew();
                all.Posts.NewUpdated -= newUpdatedHandler;
                taskCompletionSource.TrySetResult(true);
            });
            return taskCompletionSource.Task;
        }
    }
}
