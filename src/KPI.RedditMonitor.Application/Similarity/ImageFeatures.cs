using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace KPI.RedditMonitor.Application.Similarity
{
    /// <summary>
    /// Separates all pixels into buckets by their value
    /// Image Similarity preprocessing via feature histogram method
    /// </summary>
    public class ImageFeatures
    {
        private readonly Dictionary<string, Histogram> _histograms;

        public ImageFeatures()
        {
            _histograms = new Dictionary<string, Histogram>();
        }

        public Histogram AddHistogram(string name, int buckets, double minValue, double maxValue)
        {
            var histogram = new Histogram(name, buckets, minValue, maxValue);
            _histograms.Add(name, histogram);
            return histogram;
        }

        public Dictionary<string, double[]> GetNormalizedBuckets()
        {
            return _histograms.ToDictionary((kv) => kv.Key, kv => kv.Value.GetNormalizedBuckets());
        }
    }
}
