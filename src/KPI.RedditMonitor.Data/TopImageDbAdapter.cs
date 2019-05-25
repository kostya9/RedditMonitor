using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Data
{
    public class TopImageDbAdapter
    {
        private readonly ILogger<TopImageDbAdapter> _log;
        private readonly IMongoCollection<ImagePost> _collection;

        public TopImageDbAdapter(IMongoClient client, ILogger<TopImageDbAdapter> log)
        {
            _log = log;
            _collection = client.GetDatabase("reddit").GetCollection<ImagePost>("posts");
        }

        public Task Ignore(string imageUrl, bool ignoreValue)
        {
            if (imageUrl.StartsWith("https://"))
                imageUrl = imageUrl.Substring("https://".Length);

            var update = Builders<ImagePost>.Update.Set(i => i.Ignore, ignoreValue);
            return _collection.UpdateManyAsync(i => i.ImageUrl.Contains(imageUrl), update);
        }

        public async Task<List<TopImage>> GetTop(int count, bool showIgnored, string[] subreddits)
        {
            var query = _collection.Aggregate();

            if (!showIgnored)
                query = query.Match(image => image.Ignore != true);
            else
                query = query.Match(image => image.Ignore == true);

            if(subreddits?.Length > 0) 
            {
                var filter = Builders<ImagePost>.Filter.In(a => a.Subreddit, subreddits);
                query = query.Match(filter);
            }

            _log.LogInformation(query.ToString());

            var images = await query
                .Group(image => image.ImageUrl, group => new TopImage
                {
                    Count = group.Count(),
                    Url = group.Key,
                    Comments = group.Select(g => g.Url)
                })
                .Sort(new BsonDocument() { { "Count", -1 } })
                .Limit(count)
                .ToListAsync();

            foreach (var topImageDto in images)
            {
                topImageDto.EnsureUrlFormat();
            }

            // Group again gue to adjustments
            return images.GroupBy(i => i.Url).Select(g => new TopImage
            {
                Comments = g.SelectMany(gg => gg.Comments).ToArray(),
                Count = g.Sum(gg => gg.Count),
                Url = g.Key
            }).ToList();
        }

        public async Task<TopImageCount> GetCount()
        {
            var counts = await _collection.Aggregate().Group((dto) => dto.Ignore, g => new { g.Key, Count = g.Count() }).ToListAsync();
            return new TopImageCount()
            {
                All = counts.Select(c => c.Count).Sum(),
                Ignored = counts.Where(c => c.Key == true).Sum(c => c.Count)
            };
        }
    }

    public class TopImage
    {
        public void EnsureUrlFormat() 
        {
            if (!Url.StartsWith("http"))
                Url = $"https://{Url}";

            Comments = Comments.Select(c => $"https://reddit.com{c}").Distinct();
        }

        public string Url { get; set; }

        public int Count { get; set; }

        public IEnumerable<string> Comments { get; set; }
    }

    public class TopImageCount
    {
        public int All { get; set; }

        public int Ignored { get; set; }
    }
}
