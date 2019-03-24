namespace KPI.RedditMonitor.Collector.RedditPull
{
    public class RedditOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CallbackUrl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string RefreshToken { get; set; }
    }
}