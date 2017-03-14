using System;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Internal;
using EPiServer.Web.Routing.Segments;
using Geta.EpiCategories.Routing;

namespace Geta.EpiCategories.Extensions
{
    public static class RouteCollectionExtensions
    {
        public static IContentRoute MapSiteCategoryRoute(this RouteCollection routes, string name, string url, object defaults, Func<SiteDefinition, ContentReference> contentRootResolver)
        {
            return routes.MapCategoryRoute("Media_Site", name, url, defaults, contentRootResolver);
        }

        public static IContentRoute MapGlobalCategoryRoute(this RouteCollection routes, string name, string url, object defaults, Func<SiteDefinition, ContentReference> contentRootResolver)
        {
            return routes.MapCategoryRoute("Media_Global", name, url, defaults, contentRootResolver);
        }

        private static IContentRoute MapCategoryRoute(this RouteCollection routes, string insertBeforeRouteName, string name, string url, object defaults, Func<SiteDefinition, ContentReference> contentRootResolver)
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

            RouteBase mediaRoute = RouteTable.Routes[insertBeforeRouteName];
            int insertIndex = mediaRoute != null
                ? RouteTable.Routes.IndexOf(mediaRoute)
                : RouteTable.Routes.Count;

            var route = routes.MapContentRoute(name, url, defaults, contentRouteParameters) as DefaultContentRoute;
            routes.Remove(route);
            routes.Insert(insertIndex, route);
            return route;
        }
    }
}