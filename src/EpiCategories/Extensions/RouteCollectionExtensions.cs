using System;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using Geta.EpiCategories.Routing;

namespace Geta.EpiCategories.Extensions
{
    public static class RouteCollectionExtensions
    {
        public static IContentRoute MapCategoryRoute(this RouteCollection routes, string name, string url, object defaults, Func<SiteDefinition, ContentReference> contentRootResolver)
        {
            var basePathResolver = ServiceLocator.Current.GetInstance<IBasePathResolver>();
            var urlSegmentRouter = ServiceLocator.Current.GetInstance<IUrlSegmentRouter>();
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            urlSegmentRouter.RootResolver = contentRootResolver;
            Func<RequestContext, RouteValueDictionary, string> resolver = basePathResolver.Resolve;

            var contentRouteParameters = new MapContentRouteParameters
            {
                UrlSegmentRouter = urlSegmentRouter,
                BasePathResolver = resolver,
                Direction = SupportedDirection.Both,
                Constraints = new { node = new ContentTypeConstraint<CategoryData>(contentLoader) }
            };

            return routes.MapContentRoute(name, url, defaults, contentRouteParameters);
        }
    }
}