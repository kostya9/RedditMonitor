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

        public async Task<List<TopImage>> Find(Stream content)
        {
            var imageFeatures = ImageFeatureFactory.Create(content);

            var features = imageFeatures.GetBuckets();
            var images = await _repository.Get(features);
            foreach (var topImageDto in images)
            {
                topImageDto.EnsureUrlFormat();
            }

            return images;
        }
    }
}
