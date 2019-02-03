using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KPI.RedditMonitor.Data;
using KPI.RedditMonitor.ImageProcessing.Similarity;

namespace KPI.RedditMonitor.Api.Similarity
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

            var features = imageFeatures.Histograms.ToDictionary(h => h.Name, h => h.GetBuckets());
            return await _repository.Get(features);
        }
    }
}
