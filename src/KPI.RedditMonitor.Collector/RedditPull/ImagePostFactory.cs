﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KPI.RedditMonitor.Data;

namespace KPI.RedditMonitor.Collector.RedditPull
{
    public static class ImagePostFactory
    {
        private static readonly string
            imageRegexpMatch =
                @"((http|https)://)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)\.(jpg|png|bmp|jpeg|psd|tiff|svg|gif)";

        private static readonly Regex imageRegexp =
            new Regex(imageRegexpMatch, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static IEnumerable<ImagePost> Create(string id, string text, string url, DateTime createdAt, bool ignore)
        {
            var parsed = imageRegexp.Match(text);
            while (parsed.Success)
            {
                yield return new ImagePost
                {
                    Id = Guid.NewGuid().ToString("D"),
                    CreatedAt = createdAt,
                    RedditId = id,
                    Text = text,
                    ImageUrl = parsed.Value,
                    Url = url,
                    Ignore = ignore
                };

                parsed = parsed.NextMatch();
            }
        }
    }
}