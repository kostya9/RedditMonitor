﻿using System;
using System.Collections.Generic;

namespace KPI.RedditMonitor.Data
{
    public class ImagePost
    {
        public string Id { get; set; }

        public string ImageUrl { get; set; }

        public string RedditId { get; set; }

        public string Url { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Ignore { get; set; }

        public string Subreddit { get; set; }

        public Dictionary<string, double[]> FeatureBuckets { get; set; }
    }
}
