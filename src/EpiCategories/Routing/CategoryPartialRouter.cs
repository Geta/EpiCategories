using System;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;

namespace Geta.EpiCategories.Routing
{
    public class CategoryPartialRouter : IPartialRouter<PageData, CategoryData>
    {
        protected readonly IContentLoader ContentLoader;

        public CategoryPartialRouter(IContentLoader contentLoader)
        {
            ContentLoader = contentLoader;
        }

        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            var thisSegment = segmentContext.RemainingPath;
            var nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);

            while (string.IsNullOrEmpty(nextSegment.Remaining) == false)
            {
                nextSegment = segmentContext.GetNextValue(nextSegment.Remaining);
            }

            if (string.IsNullOrWhiteSpace(nextSegment.Next) == false)
            {
                var category = ContentLoader.GetBySegment(ContentReference.RootPage, nextSegment.Next, LanguageSelector.Fallback(content.LanguageBranch, true)) as CategoryData;

                if (category == null) return null;

                segmentContext.RemainingPath = thisSegment.Substring(0, thisSegment.LastIndexOf(nextSegment.Next, StringComparison.InvariantCultureIgnoreCase));
                segmentContext.RouteData.Values.Add(CategoryRoutingConstants.CurrentCategory, category);
                return content;
            }

            return null;
        }

        public PartialRouteData GetPartialVirtualPath(CategoryData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return null;
        }
    }
}