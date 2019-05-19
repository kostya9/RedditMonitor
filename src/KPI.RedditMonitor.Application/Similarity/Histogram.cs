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

        private readonly double _step;

        public Histogram(string name, int buckets, double minValue, double maxValue)
        {
            Name = name;
            _minValue = minValue;
            _maxValue = maxValue;

            _buckets = new int[buckets];

            _step = (_maxValue - _minValue + 1) / _buckets.Length;
        }

        public void Add(double value)
        {
            if (value > _maxValue || value < _minValue)
                throw new ArgumentException($"Value {value} is out of bounds");

            _buckets[GetValueIndex(value)]++;
        }

        public double[] GetBuckets()
        {
            var total = _buckets.Sum();
            var normalized = _buckets.Select(b => b / (double)total).ToArray();
            return normalized;
        }

        private int GetValueIndex(double value)
        {
            return (int)((value - _minValue) / _step);
        }
    }
}
