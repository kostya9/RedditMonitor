using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace KPI.RedditMonitor.Data
{
    public class ImagePostsRepository
    {
        private readonly IMongoCollection<ImagePost> _collection;

        public ImagePostsRepository(MongoClient client)
        {
            _collection = client
                .GetDatabase("reddit")
                .GetCollection<ImagePost>("posts");
        }

        public async Task AddRange(IEnumerable<ImagePost> posts)
        {
            await _collection.InsertManyAsync(posts);
        }
    }
}
