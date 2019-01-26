using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KPI.RedditMonitor.Data;

namespace KPI.RedditMonitor.Collector
{
    public static class ImagePostFactory
    {
        private static string 
            imageRegexpMatch = @"((http|https)://)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)\.(jpg|png|bmp|jpeg|psd|tiff|svg|gif)";

        private static Regex imageRegexp = new Regex(imageRegexpMatch, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static IEnumerable<ImagePost> Create(string id, string text, string url)
        {
            var parsed = imageRegexp.Match(text);
            while (parsed.Success)
            {
                yield return new ImagePost
                {
                    Id = Guid.NewGuid().ToString("D"),
                    CreatedAt = DateTime.UtcNow,
                    RedditId = id,
                    Text = text,
                    ImageUrl = parsed.Value,
                    Url = url
                };

                parsed = parsed.NextMatch();
            }
        }
    }
}
