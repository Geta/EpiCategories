using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.Search;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.Search;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Search;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Geta.EpiCategories.Search
{
    [SearchProvider]
    public class CategoriesSearchProvider : EPiServerSearchProviderBase<CategoryData, ContentType>
    {
        private readonly LocalizationService _localizationService;
        private readonly IEnumerable<IContentRepositoryDescriptor> _contentRepositoryDescriptors;

        public CategoriesSearchProvider(LocalizationService localizationService, ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository<ContentType> contentTypeRepository, EditUrlResolver editUrlResolver, ServiceAccessor<SiteDefinition> currentSiteDefinition, IContentRepository contentRepository, ILanguageBranchRepository languageBranchRepository, SearchHandler searchHandler, ContentSearchHandler contentSearchHandler, SearchIndexConfig searchIndexConfig, UIDescriptorRegistry uiDescriptorRegistry, LanguageResolver languageResolver, UrlResolver urlResolver, TemplateResolver templateResolver, IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors) : base(localizationService, siteDefinitionResolver, contentTypeRepository, editUrlResolver, currentSiteDefinition, contentRepository, languageBranchRepository, searchHandler, contentSearchHandler, searchIndexConfig, uiDescriptorRegistry, languageResolver, urlResolver, templateResolver)
        {
            _localizationService = localizationService;
            _contentRepositoryDescriptors = contentRepositoryDescriptors;
        }

        public override string Area => "CMS/categories";

        public override string Category => _localizationService.GetString("/cms/searchprovider/categories/name", "Categories");

        protected override string IconCssClass => "epi-resourceIcon epi-resourceIcon-category";

        public override IEnumerable<SearchResult> Search(Query query)
        {
            query.SearchRoots = _contentRepositoryDescriptors
                .First(x => x.Key == CategoryContentRepositoryDescriptor.RepositoryKey)
                .Roots
                .Select(x => x.ToReferenceWithoutVersion().ToString());

            return base.Search(query);
        }
    }
}