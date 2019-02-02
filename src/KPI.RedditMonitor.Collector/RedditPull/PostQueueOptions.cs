namespace KPI.RedditMonitor.Collector.RedditPull
{
    public class PostQueueOptions
    {
        public string QueueUrl { get; set; }

        public int BatchDelay { get; set; } = 30;
    }
}