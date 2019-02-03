﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace KPI.RedditMonitor.ImageProcessing.Similarity
{ 
    public class Histogram
    {
        public string Name { get; }

        private readonly double _minValue;
        private readonly double _maxValue;
        private readonly List<double>[] _buckets;

        private int _total;

        public Histogram(string name, int buckets, double minValue, double maxValue)
        {
            Name = name;
            _minValue = minValue;
            _maxValue = maxValue;

            _buckets = new List<double>[buckets];

            for (int i = 0; i < _buckets.Length; i++)
            {
                _buckets[i] = new List<double>();
            }

            _total = 0;
        }

        public void Add(double value)
        {
            if (value > _maxValue || value < _minValue)
                throw new ArgumentException($"Value {value} is out of bounds");

            _buckets[GetValueIndex(value)].Add(value);
            _total++;
        }

        public double[] GetBuckets()
        {
            var normalized = _buckets.Select(b => b.Count / (double)_total).ToArray();
            return normalized;
        }

        private int GetValueIndex(double value)
        {
            var step = (_maxValue - _minValue) / _buckets.Length;

            for (var i = 0; i < _buckets.Length; i++)
            {
                if (value <= _minValue + step * (i + 1))
                    return i;
            }

            return _buckets.Length - 1;
        }
    }
}