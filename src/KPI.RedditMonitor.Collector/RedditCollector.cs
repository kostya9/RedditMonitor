using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;

namespace KPI.RedditMonitor.Collector
{
    public class RedditCollector
    {
        private readonly Reddit _reddit;

        public RedditCollector(RedditOptions options)
        {
            var webAgent = new BotWebAgent(options.Username, options.Password, options.ClientId,
                options.ClientSecret, options.CallbackUrl);
            _reddit = new Reddit(webAgent);
        }

        public Task SubscribeOnEntries(Action<RedditPost> callback, CancellationToken token = default)
        {
            var comments = _reddit.RSlashAll.GetComments(limitPerRequest: 100).Stream();
            var posts = _reddit.RSlashAll.GetPosts().Stream();

            comments.ForEachAsync((t) => 
                callback(new RedditPost(t.Id, t.Body, t.Permalink.ToString())), token);
            posts.ForEachAsync((t) => 
                callback(new RedditPost(t.Id, t.Title + " " + t.SelfText + " " + t.Url.AbsoluteUri, t.Permalink.ToString())), token);

            return Task.WhenAll(comments.Enumerate(token), posts.Enumerate(token));
        }
    }

    public class RedditPost
    {
        public RedditPost(string id, string text, string url)
        {
            Id = id;
            Text = text;
            Url = url;
        }

        public string Id { get; }

        public string Text { get; }

        public string Url { get; set; }
    }
}
