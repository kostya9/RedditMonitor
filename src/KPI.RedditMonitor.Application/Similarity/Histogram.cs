using System;
using System.Linq;

namespace KPI.RedditMonitor.Application.Similarity
{
    public class Histogram
    {
        public string Name { get; }

        private readonly double _minValue;
        private readonly double _maxValue;
        private readonly int[] _buckets;

        public Histogram(string name, int buckets, double minValue, double maxValue)
        {
            Name = name;
            _minValue = minValue;
            _maxValue = maxValue;

            _buckets = new int[buckets];
        }

        public void Add(double value)
        {
            if (value > _maxValue || value < _minValue)
                throw new ArgumentException($"Value {value} is out of bounds");

            _buckets[GetValueIndex(value)]++;
        }

        public double[] GetBuckets(int totalPixels)
        {
            var normalized = _buckets.Select(b => b / (double)totalPixels).ToArray();
            return normalized;
        }

        private int GetValueIndex(double value)
        {
            var step = (_maxValue - _minValue + 1) / _buckets.Length;
            var idx = (int)((value - _minValue) / step);
            return idx;
        }
    }
}
