using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditSharp;
using RedditSharp.Things;

namespace KPI.RedditMonitor.Collector.RedditPull.Collectors
{
    public class RedditSharpCollector : IRedditCollector
    {
        private readonly ILogger<RedditSharpCollector> _log;
        private readonly RedditOptions _options;

        public RedditSharpCollector(IOptions<RedditOptions> options, ILogger<RedditSharpCollector> log)
        {
            _options = options.Value;
            _log = log;
        }

        public async Task SubscribeOnEntries(Action<RedditPost> callback, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            using (var source = new CancellationTokenSource(TimeSpan.FromHours(112)))
            {

                cancellationToken.Register(() => source.Cancel());
                var webAgent = new BotWebAgent(_options.Username, _options.Password, _options.ClientId,
                    _options.ClientSecret, _options.CallbackUrl)
                {
                    UserAgent = $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription}:RedditMonitor:v1.0.0 (by /u/{_options.Username})"
                };
                var reddit = new RedditSharp.Reddit(webAgent, false);

                var comments = reddit.RSlashAll.GetComments(limitPerRequest: 100).Stream();
                var posts = reddit.RSlashAll.GetPosts(Subreddit.Sort.New).Stream();

                comments.ForEachAsync(t =>
                    callback(new RedditPost(t.Id, t.Body, t.Permalink.ToString(), t.CreatedUTC,
                        t.IsStickied || t.Distinguished != ModeratableThing.DistinguishType.None)), source.Token);
                posts.ForEachAsync(t =>
                    callback(new RedditPost(t.Id, t.Title + " " + t.SelfText + " " + t.Url.AbsoluteUri,
                        t.Permalink.ToString(), t.CreatedUTC, t.NSFW)), source.Token);
                try
                {
                    await Task.WhenAll(comments.Enumerate(source.Token), posts.Enumerate(source.Token));
                }
                catch (OperationCanceledException)
                {
                    if (!cancellationToken.IsCancellationRequested)
                        _log.LogInformation("Reconnecting to reddit...");
                }
            }
        }
    }
}