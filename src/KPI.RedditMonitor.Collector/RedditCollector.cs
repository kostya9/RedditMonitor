using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedditSharp;
using RedditSharp.Things;

namespace KPI.RedditMonitor.Collector
{
    public class RedditCollector
    {
        private readonly RedditOptions _options;
        private readonly ILogger<RedditCollector> _log;

        public RedditCollector(RedditOptions options, ILogger<RedditCollector> log)
        {
            _options = options;
            _log = log;
        }

        public async Task SubscribeOnEntries(Action<RedditPost> callback)
        {
            while (true)
            {
                var source = new CancellationTokenSource(TimeSpan.FromHours(1));

                var webAgent = new BotWebAgent(_options.Username, _options.Password, _options.ClientId,
                    _options.ClientSecret, _options.CallbackUrl)
                {
                    UserAgent = _options.UserAgent
                };
                var reddit = new Reddit(webAgent);

                var comments = reddit.RSlashAll.GetComments(limitPerRequest: 100).Stream();
                var posts = reddit.RSlashAll.GetPosts(Subreddit.Sort.New).Stream();

                comments.ForEachAsync((t) =>
                    callback(new RedditPost(t.Id, t.Body, t.Permalink.ToString(), t.CreatedUTC,
                        t.IsStickied || t.Distinguished != ModeratableThing.DistinguishType.None)), source.Token);
                posts.ForEachAsync((t) =>
                    callback(new RedditPost(t.Id, t.Title + " " + t.SelfText + " " + t.Url.AbsoluteUri, t.Permalink.ToString(), t.CreatedUTC, t.NSFW)), source.Token);

                try
                {
                    await Task.WhenAll(comments.Enumerate(source.Token), posts.Enumerate(source.Token));
                }
                catch (OperationCanceledException)
                {
                    _log.LogInformation("Reconnecting to reddit...");
                }
            }
        }
    }

    public class RedditPost
    {
        public RedditPost(string id, string text, string url, DateTime createdAt, bool ignore)
        {
            Id = id;
            Text = text;
            Url = url;
            CreatedAt = createdAt;
            Ignore = ignore;
        }

        public string Id { get; }

        public string Text { get; }

        public string Url { get; }

        public DateTime CreatedAt { get; }

        public bool Ignore { get; }
    }
}
