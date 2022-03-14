using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.Web;

namespace Geta.EpiCategories.Extensions
{
    public static class IContentRepositoryExtensions
    {
        public static ContentReference GetOrCreateGlobalCategoriesRoot(this IContentRepository contentRepository)
        {
            var siteAssetsExists = SiteDefinition.Current.GlobalAssetsRoot != SiteDefinition.Current.SiteAssetsRoot;
            var name = siteAssetsExists ? "For All Sites" : "Categories";
            var routeSegment = siteAssetsExists ? "global-categories" : "categories";
            return contentRepository.GetOrCreateCategoriesRoot(SiteDefinition.Current.GlobalAssetsRoot, name, routeSegment);
        }

        public static ContentReference GetOrCreateSiteCategoriesRoot(this IContentRepository contentRepository)
        {
            var siteAssetsExists = SiteDefinition.Current.GlobalAssetsRoot != SiteDefinition.Current.SiteAssetsRoot;
            var name = siteAssetsExists ? "For This Site" : "Categories";
            var routeSegment = "categories";
            return contentRepository.GetOrCreateCategoriesRoot(SiteDefinition.Current.SiteAssetsRoot, name, routeSegment);
        }

        private static ContentReference GetOrCreateCategoriesRoot(this IContentRepository contentRepository, ContentReference parentLink, string name, string routeSegment)
        {
            var loaderOptions = new LoaderOptions
            {
                LanguageLoaderOption.FallbackWithMaster()
            };

            var rootCategory = contentRepository.GetChildren<CategoryRoot>(parentLink, loaderOptions).OrderBy(root => root.ContentLink.ID).FirstOrDefault();

            if (rootCategory != null)
            {
                return rootCategory.ContentLink;
            }

            rootCategory = contentRepository.GetDefault<CategoryRoot>(parentLink);
            rootCategory.Name = name;
            rootCategory.RouteSegment = routeSegment;
            return contentRepository.Save(rootCategory, SaveAction.Publish, AccessLevel.NoAccess);
        }
    }
}