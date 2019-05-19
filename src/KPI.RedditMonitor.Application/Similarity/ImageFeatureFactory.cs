using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;

namespace KPI.RedditMonitor.Application.Similarity
{
    public static class ImageFeatureFactory
    {
        public static ImageFeatures Create(Stream content)
        {
            var histograms = new ImageFeatures();
            using (var image = Image.Load(content))
            {
                var buckets = 4;
                var minRgb = 0;
                var maxRgb = 255;
                var red = histograms.AddHistogram("red", buckets, minRgb, maxRgb);
                var green = histograms.AddHistogram("green", buckets, minRgb, maxRgb);
                var blue = histograms.AddHistogram("blue", buckets, minRgb, maxRgb);

                foreach (var pixel in image.GetPixelSpan())
                {
                    red.Add(pixel.R);
                    green.Add(pixel.G);
                    blue.Add(pixel.B);
                }
            }

            return histograms;
        }
    }
}
