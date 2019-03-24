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
