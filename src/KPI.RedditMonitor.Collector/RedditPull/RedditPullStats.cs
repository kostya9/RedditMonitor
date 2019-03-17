using System;

namespace KPI.RedditMonitor.Collector.RedditPull
{
    public class RedditPullStats
    {
        public int Images { get; private set; }
        public int Seconds { get; private set; }

        internal void RegisterBatch(int images, int seconds)
        {
            Images = images;
            Seconds = seconds;
        }
    }
}
