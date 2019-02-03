using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;

namespace KPI.RedditMonitor.ImageProcessing.Similarity
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
                var red = histograms.Add("red", buckets, minRgb, maxRgb);
                var green = histograms.Add("green", buckets, minRgb, maxRgb);
                var blue = histograms.Add("blue", buckets, minRgb, maxRgb);

                foreach (var pixel in image.GetPixelSpan())
                {
                    red.Add(pixel.R);
                    blue.Add(pixel.B);
                    green.Add(pixel.G);
                }
            }

            return histograms;
        }
    }
}
