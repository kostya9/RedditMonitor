using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace KPI.RedditMonitor.Data
{
    public class ImagePostsRepository
    {
        private readonly IMongoClient _client;

        public ImagePostsRepository(IMongoClient client)
        {
            _client = client;
        }

        private IMongoCollection<ImagePost> GetCollection() 
        {
            return _client.GetDatabase("reddit")
                .GetCollection<ImagePost>("posts");
        }

        public async Task AddRange(IEnumerable<ImagePost> posts)
        {
            await GetCollection().InsertManyAsync(posts);
        }

        public async Task Add(ImagePost imagePost)
        {
            await GetCollection().InsertOneAsync(imagePost);
        }

        /// <summary>
        /// Algorithm uses Intersection to compare color histograms
        /// </summary>
        /// <param name="features"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<List<ImagePost>> Get(Dictionary<string, double[]> features, int top = 10)
        {
            var reds = BsonArray.Create(features["red"]);
            var blues = BsonArray.Create(features["blue"]);
            var greens = BsonArray.Create(features["green"]);

            var query = @"
[{
        $match: {
            Ignore: {
                '$in': [null, false]
            }
        }
    },
    {
        $addFields: {
            reds: {
                $zip: {
                    inputs: [" + reds + @", '$FeatureBuckets.red']
                }
            },
            blues: {
                $zip: {
                    inputs: [" + blues + @", '$FeatureBuckets.blue']
                }
            },
            greens: {
                $zip: {
                    inputs: [" + greens + @", '$FeatureBuckets.green']
                }
            }
        }
    },
    {
        $addFields: {
            allFeatures: {
                $concatArrays: ['$greens', '$reds', '$blues']
            }
        }
    },
    {
        $unwind: {
            path: '$allFeatures'
        }
    },
    {
        $addFields: {
            diff: {
                $min: '$allFeatures'
            }
        }
    },
    {
        $group: {
            _id: '$_id',
            totalDistance: {
                $sum: '$diff'
            },
            image: {
                '$first': '$$ROOT'
            }
        }
    },
    {
        $group: {
            _id: '$image.ImageUrl',
            image: {
                '$first': '$image'
            },
            totalDistance: {
                '$first': '$totalDistance'
            }
        }
    },
    {
        $sort: {
            'totalDistance': 1
        }
    },
    {
        $limit: 5
    }
]
";
            var pipelines = BsonSerializer.Deserialize<BsonDocument[]>(query).ToList();

            using (var result = await GetCollection().AggregateAsync<AggregateResult>(pipelines, new AggregateOptions()
            {
                AllowDiskUse = true
            }))
            {
                await result.MoveNextAsync();
                return result.Current.Select(c => c.image).ToList();
            }
        }
        
        private class AggregateResult
        {
            public string Id { get; set; }

            public double totalDistance { get; set; }

            public ImagePost image { get; set; }
        }

    }
}
