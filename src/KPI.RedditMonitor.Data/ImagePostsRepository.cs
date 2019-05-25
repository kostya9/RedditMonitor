﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace KPI.RedditMonitor.Data
{
    public class ImagePostsRepository
    {
        private readonly ILogger<ImagePostsRepository> _log;
        private readonly IMongoClient _client;

        public ImagePostsRepository(IMongoClient client, ILogger<ImagePostsRepository> log)
        {
            _log = log;
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
        public async Task<List<TopImage>> Get(Dictionary<string, double[]> features, int top = 10)
        {
            var featuresArray = features["red"].Concat(features["green"]).Concat(features["blue"]);
            var featuresBson = BsonArray.Create(featuresArray);

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
            allFeatures: {
                $zip: {
                    inputs: [" + featuresBson + @", {$concatArrays: ['$FeatureBuckets.red', '$FeatureBuckets.green', '$FeatureBuckets.blue']}]
                }
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
            intersection: {
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
            comments: {
                '$addToSet': '$image.Url'
            },
            intersection: {
                '$first': '$intersection'
            }
        }
    },
    {
        $sort: {
            'intersection': -1
        }
    },
    {
        $limit: 5
    }
]
";

            _log.LogInformation(query);
            Console.WriteLine(query);
            var pipelines = BsonSerializer.Deserialize<BsonDocument[]>(query).ToList();

            using (var result = await GetCollection().AggregateAsync<AggregateResult>(pipelines, new AggregateOptions()
            {
                AllowDiskUse = true
            }))
            {
                await result.MoveNextAsync();
                return result.Current.Select(c => new TopImage
                {
                    Comments = c.comments,
                    Count = c.comments.Length,
                    Url = c.image.ImageUrl
                }).ToList();
            }
        }
        
        private class AggregateResult
        {
            public string Id { get; set; }

            public double intersection { get; set; }

            public ImagePost image { get; set; }

            public string[] comments { get; set; }
        }

    }
}
