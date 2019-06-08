using BenchmarkDotNet.Attributes;
using KPI.RedditMonitor.Application.Similarity;
using System;
using System.IO;

namespace KPI.RedditMonitor.Benchmarks
{
    [MemoryDiagnoser]
    public class ImageExtractionBenchmark
    {
        [Params("800x1086-101KB.jpg", "1125x1500-74KB.jpg", "2060x1920-765KB.jpg", "4032x3024-2681KB.jpg")]
        public string Image { get; set; }

        private Stream _file;

        [IterationSetup]
        public void ReadStuff()
        {
            _file = File.OpenRead($"./img/{Image}");
        }

        [IterationCleanup]
        public void DisposeStuff()
        {
            _file.Dispose();
        }

        [Benchmark]
        public void ExtractFeatures()
        {
            ImageFeatureFactory.Create(_file);
        }
    }
}
