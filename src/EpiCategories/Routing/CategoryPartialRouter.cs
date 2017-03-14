using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using EPiServer.ServiceLocation;

namespace Geta.EpiCategories.Routing
{
    [ServiceConfiguration(typeof(CategoryPartialRouter), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CategoryPartialRouter : IPartialRouter<ICategoryRoutableContent, ICategoryRoutableContent>
    {
        protected readonly IContentLoader ContentLoader;
        protected readonly ICategoryContentLoader CategoryLoader;
        protected readonly LanguageResolver LanguageResolver;
        public static string CategorySeparator = ConfigurationManager.AppSettings["GetaEpiCategories:CategorySeparator"] ?? "__";

        public CategoryPartialRouter(IContentLoader contentLoader, ICategoryContentLoader categoryLoader, LanguageResolver languageResolver)
        {
            ContentLoader = contentLoader;
            CategoryLoader = categoryLoader;
            LanguageResolver = languageResolver;
        }

        public object RoutePartial(ICategoryRoutableContent content, SegmentContext segmentContext)
        {
            var thisSegment = segmentContext.RemainingPath;
            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);

            while (string.IsNullOrEmpty(nextSegment.Remaining) == false)
            {
                nextSegment = segmentContext.GetNextValue(nextSegment.Remaining);
            }

            if (string.IsNullOrWhiteSpace(nextSegment.Next) == false)
            {
                var localizableContent = content as ILocale;
                CultureInfo preferredCulture = localizableContent?.Language ?? ContentLanguage.PreferredCulture;

                string[] categoryUrlSegments = nextSegment.Next.Split(new [] { CategorySeparator }, StringSplitOptions.RemoveEmptyEntries);
                var categories = new List<CategoryData>();

                foreach (var categoryUrlSegment in categoryUrlSegments)
                {
                    var category = CategoryLoader.GetFirstBySegment<CategoryData>(categoryUrlSegment, preferredCulture);

                    if (category == null)
                        return null;

                    categories.Add(category);
                }

                segmentContext.RemainingPath = thisSegment.Substring(0, thisSegment.LastIndexOf(nextSegment.Next, StringComparison.InvariantCultureIgnoreCase));
                segmentContext.SetCustomRouteData(CategoryRoutingConstants.CurrentCategory, categories[0]);
                segmentContext.SetCustomRouteData(CategoryRoutingConstants.CurrentCategories, categories);
                segmentContext.RoutedContentLink = content.ContentLink;
                segmentContext.RoutedObject = content;

                return content;
            }

            return null;
        }

        public PartialRouteData GetPartialVirtualPath(ICategoryRoutableContent content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            if (requestContext.IsInEditMode())
            {
                return null;
            }

            // Single category
            object currentCategory;
            if (routeValues.TryGetValue(CategoryRoutingConstants.CurrentCategory, out currentCategory))
            {
                ContentReference categoryLink = currentCategory as ContentReference;

                if (ContentReference.IsNullOrEmpty(categoryLink))
                    return null;

                CategoryData category;

                if (CategoryLoader.TryGet(categoryLink, out category) == false)
                    return null;

                // Remove from query now that it's handled.
                routeValues.Remove(CategoryRoutingConstants.CurrentCategory);

                return new PartialRouteData
                {
                    BasePathRoot = content.ContentLink,
                    PartialVirtualPath = $"{category.RouteSegment}/"
                };
            }

            // Multiple categories
            object currentCategories;
            if (routeValues.TryGetValue(CategoryRoutingConstants.CurrentCategories, out currentCategories))
            {
                var categoryContentLinks = currentCategories as IEnumerable<ContentReference>;

                if (categoryContentLinks == null)
                    return null;

                var categorySegments = new List<string>();

                foreach (var categoryContentLink in categoryContentLinks)
                {
                    CategoryData category;
                    if (ContentLoader.TryGet(categoryContentLink, out category) == false)
                        return null;

                    categorySegments.Add(category.RouteSegment);
                }

                categorySegments.Sort(StringComparer.Create(LanguageResolver.GetPreferredCulture(), true));

                // Remove from query now that it's handled.
                routeValues.Remove(CategoryRoutingConstants.CurrentCategories);

                return new PartialRouteData
                {
                    BasePathRoot = content.ContentLink,
                    PartialVirtualPath = $"{string.Join(CategoryPartialRouter.CategorySeparator, categorySegments)}/"
                };
            }

            return null;
        }
    }
}