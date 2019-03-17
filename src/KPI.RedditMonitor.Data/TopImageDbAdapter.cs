using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Data
{
    public class TopImageDbAdapter
    {
        private readonly IMongoCollection<ImagePost> _collection;

        public TopImageDbAdapter(IMongoClient client)
        {
            _collection = client.GetDatabase("reddit").GetCollection<ImagePost>("posts");
        }

        public Task Ignore(string imageUrl, bool ignoreValue)
        {
            if (imageUrl.StartsWith("https://"))
                imageUrl = imageUrl.Substring("https://".Length);

            var update = Builders<ImagePost>.Update.Set(i => i.Ignore, ignoreValue);
            return _collection.UpdateManyAsync(i => i.ImageUrl.Contains(imageUrl), update);
        }

        public async Task<List<TopImage>> GetTop(int count, bool showIgnored)
        {
            var query = _collection.Aggregate();

            if (!showIgnored)
                query = query.Match(image => image.Ignore != true);
            else
                query = query.Match(image => image.Ignore == true);

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
                if (!topImageDto.Url.StartsWith("http"))
                    topImageDto.Url = $"https://{topImageDto.Url}";

                topImageDto.Comments = topImageDto.Comments.Select(c => $"https://reddit.com{c}");
            }

            return images;
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
        public string Url { get; set; }

        public string CdnUrl { get; set; }

        public int Count { get; set; }

        public IEnumerable<string> Comments { get; set; }
    }

    public class TopImageCount
    {
        public int All { get; set; }

        public int Ignored { get; set; }
    }
}
