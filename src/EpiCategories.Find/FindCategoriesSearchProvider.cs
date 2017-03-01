using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Find.Cms.SearchProviders;
using EPiServer.Framework.Localization;
using EPiServer.Shell;
using EPiServer.Shell.Search;
using EPiServer.Web;

namespace Geta.EpiCategories.Find
{
    [SearchProvider]
    public class FindCategoriesSearchProvider : EnterpriseContentSearchProviderBase<CategoryData, ContentType>
    {
        private readonly LocalizationService _localizationService;
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;

        public FindCategoriesSearchProvider(LocalizationService localizationService, ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository contentTypeRepository, UIDescriptorRegistry uiDescriptorRegistry) : base(localizationService, siteDefinitionResolver, contentTypeRepository, uiDescriptorRegistry)
        {
            _localizationService = localizationService;
            _siteDefinitionResolver = siteDefinitionResolver;
        }

        protected override string ToolTipResourceKeyBase => "/shell/cms/search/pages/tooltip";

        protected override string ToolTipContentTypeNameResourceKey => "contenttype";

        public override string Area => "CMS/categories";

        public override string Category => _localizationService.GetString("/cms/searchprovider/findcategories/name", "Find categories");

        protected override string GetEditUrl(CategoryData contentData, out bool onCurrentHost)
        {
            ContentReference contentLink = contentData.ContentLink;
            string language = contentData.Language.Name;
            string editPath = EditPath(contentData, contentLink, language);
            onCurrentHost = true;

            var contentSiteDefinition = _siteDefinitionResolver.GetByContent(contentLink, true);

            if (contentSiteDefinition != null && contentSiteDefinition.SiteUrl == SiteDefinition.Current.SiteUrl)
                return editPath;

            onCurrentHost = false;
            return editPath;
        }

        protected override string IconCssClass(CategoryData contentData)
        {
            return "epi-resourceIcon epi-resourceIcon-category";
        }
    }
}