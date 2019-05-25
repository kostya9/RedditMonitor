using System;
using System.Threading;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Collector.RedditPull.Collectors
{
    public interface IRedditCollector
    {
        Task SubscribeOnEntries(Action<RedditPost> callback, CancellationToken cancellationToken);
    }

    public class RedditPost
    {
        public RedditPost(string id, string text, string url, DateTime createdAt, bool ignore, string subreddit)
        {
            Id = id;
            Text = text;
            Url = url;
            CreatedAt = createdAt;
            Ignore = ignore;
            Subreddit = subreddit;
        }

        public string Id { get; }

        public string Text { get; }

        public string Url { get; }

        public DateTime CreatedAt { get; }

        public bool Ignore { get; }
        
        public string Subreddit { get; }
    }
}
