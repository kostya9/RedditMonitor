using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KPI.RedditMonitor.Data;

namespace KPI.RedditMonitor.Application.Similarity
{
    public class SimilarityService
    {
        private readonly ImagePostsRepository _repository;

        public SimilarityService(ImagePostsRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ImagePost>> Find(Stream content)
        {
            var imageFeatures = ImageFeatureFactory.Create(content);

            var features = imageFeatures.GetBuckets();
            return await _repository.Get(features);
        }
    }
}
