using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;

namespace KPI.RedditMonitor.Application.Similarity
{
    public static class ImageFeatureFactory
    {
        public static ImageFeatures Create(Stream content)
        {
            var features = new ImageFeatures();
            using (var image = Image.Load(content))
            {
                const int buckets = 4;
                const int minRgb = 0;
                const int maxRgb = 255;

                var red = features.AddHistogram("red", buckets, minRgb, maxRgb);
                var green = features.AddHistogram("green", buckets, minRgb, maxRgb);
                var blue = features.AddHistogram("blue", buckets, minRgb, maxRgb);

                foreach (var pixel in image.GetPixelSpan())
                {
                    red.Add(pixel.R);
                    green.Add(pixel.G);
                    blue.Add(pixel.B);
                }
            }

            return features;
        }
    }
}
