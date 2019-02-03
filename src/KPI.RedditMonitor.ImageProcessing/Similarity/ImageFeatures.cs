using System;
using System.Collections.Generic;

namespace KPI.RedditMonitor.ImageProcessing.Similarity
{
    /// <summary>
    /// Separates all pixels into buckets by their value
    /// Image Similarity preprocessing vie feature histogram method
    /// </summary>
    public class ImageFeatures
    {
        private readonly List<Histogram> _histograms;

        public IReadOnlyCollection<Histogram> Histograms => _histograms;

        public ImageFeatures()
        {
            _histograms = new List<Histogram>();
        }

        public Histogram Add(string name, int buckets, double minValue, double maxValue)
        {
            var histogram = new Histogram(name, buckets, minValue, maxValue);
            _histograms.Add(histogram);
            return histogram;
        }

        public double DistanceTo(ImageFeatures other)
        {
            if (other._histograms.Count != _histograms.Count)
                throw new ArgumentException("Please provide features of the same length");

            double distance = 0;

            for (int i = 0; i < _histograms.Count; i++)
            {
                var buckets = _histograms[i].GetBuckets();
                var otherBuckets = other._histograms[i].GetBuckets();

                if (buckets.Length != otherBuckets.Length)
                    throw new ArgumentException("Please provide buckets of the same length");

                for (int j = 0; j < buckets.Length; j++)
                {
                    distance += Math.Abs(buckets[j] - otherBuckets[j]);
                }
            }

            return distance;
        }
    }
}
