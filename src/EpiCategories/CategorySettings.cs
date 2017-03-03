using EPiServer;
using EPiServer.ServiceLocation;
using Geta.EpiCategories.Extensions;

namespace Geta.EpiCategories
{
    [ServiceConfiguration(typeof(CategorySettings), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CategorySettings
    {
        private readonly IContentRepository _contentRepository;

        public CategorySettings(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public int GlobalCategoriesRoot => _contentRepository.GetOrCreateGlobalCategoriesRoot().ID;
        public int SiteCategoriesRoot => _contentRepository.GetOrCreateSiteCategoriesRoot().ID;
    }
}