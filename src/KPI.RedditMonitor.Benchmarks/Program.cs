using BenchmarkDotNet.Running;
using System;

namespace KPI.RedditMonitor.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ImageExtractionBenchmark>();
        }
    }
}
