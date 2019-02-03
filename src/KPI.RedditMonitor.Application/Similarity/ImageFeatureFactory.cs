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
                histograms.AddHistogram("red", buckets, minRgb, maxRgb);
                histograms.AddHistogram("green", buckets, minRgb, maxRgb);
                histograms.AddHistogram("blue", buckets, minRgb, maxRgb);

                foreach (var pixel in image.GetPixelSpan())
                {
                    histograms.AddPixel("red", pixel.R);
                    histograms.AddPixel("blue", pixel.B);
                    histograms.AddPixel("green", pixel.G);
                }
            }

            return histograms;
        }
    }
}
