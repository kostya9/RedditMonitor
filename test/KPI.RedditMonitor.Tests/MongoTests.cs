using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KPI.RedditMonitor.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Xunit;

namespace KPI.RedditMonitor.Tests
{
    public class MongoTests
    {
        private readonly ImagePostsRepository _repository;

        public MongoTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.Development.json", true)
                .AddEnvironmentVariables()
                .Build();
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("ignoreExtra", conventionPack, t => true);

            var mongoClient = new MongoClient(config["MongoDb:ConnectionString"]);

            _repository = new ImagePostsRepository(mongoClient, NullLogger<ImagePostsRepository>.Instance);
        }

        [Fact]
        public async Task GetTopTest()
        {
            var top = await _repository.Get(new Dictionary<string, double[]>()
            {
                {"red", new[] {0.2, 0.3, 0.2, 0.3}},
                {"green", new[] {0.2, 0.3, 0.2, 0.3}},
                {"blue", new[] {0.2, 0.3, 0.2, 0.3}}
            });

            Assert.True(top.Count == 5);
        }
    }
}
