using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KPI.RedditMonitor.Collector.RedditPull;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditSharp;

namespace KPI.RedditMonitor 
{
    public class RedditPostsService
    {
        private readonly RedditOptions _options;
        private readonly ILogger<RedditPostsService> _log;
        private RedditSharp.Reddit _reddit;

        public RedditPostsService(IOptions<RedditOptions> options, ILogger<RedditPostsService> log)
        {
            _options = options.Value;
            _log = log;

            var webAgent = new BotWebAgent(_options.Username, _options.Password, _options.ClientId,
                _options.ClientSecret, _options.CallbackUrl)
            {
                UserAgent = $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription}:RedditMonitor:v1.0.0 (by /u/{_options.Username})"
            };
            _reddit = new RedditSharp.Reddit(webAgent, false);
        }

        public async Task<RedditPost> GetPost(string url)
        {
            var post = await _reddit.GetPostAsync(new System.Uri(url));

            if(post == null)
                return null;

            return new RedditPost
            {
                FullUrl = url,
                Title = post.Title,
                Text = post.SelfText
            };
        }
    }

    public class RedditPost 
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string FullUrl { get; set; }
    }
}