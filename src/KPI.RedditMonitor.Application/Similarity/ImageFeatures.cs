using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace KPI.RedditMonitor.Application.Similarity
{
    /// <summary>
    /// Separates all pixels into buckets by their value
    /// Image Similarity preprocessing vie feature histogram method
    /// </summary>
    public class ImageFeatures
    {
        private readonly Dictionary<string, Histogram> _histograms;
        private int _total;

        public ImageFeatures()
        {
            _histograms = new Dictionary<string, Histogram>();
        }

        public void AddHistogram(string name, int buckets, double minValue, double maxValue)
        {
            var histogram = new Histogram(name, buckets, minValue, maxValue);
            _histograms.Add(name, histogram);
        }

        public void AddPixel(string name, double value)
        {
            _histograms[name].Add(value);
            _total++;
        }

        public Dictionary<string, double[]> GetBuckets()
        {
            return _histograms.ToDictionary((kv) => kv.Key, kv => kv.Value.GetBuckets(_total));
        }
    }
}
