using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Geta.EpiCategories.Routing;

namespace Geta.EpiCategories.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ContentUrl(this UrlHelper url, ContentReference contentLink, object routeValues)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

            return urlResolver.GetVirtualPath(contentLink, null, new VirtualPathArguments {RouteValues = new RouteValueDictionary(routeValues)}).GetUrl();
        }

        public static string CategoryRoutableContentUrl(this UrlHelper url, ContentReference contentLink, ContentReference categoryLink)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

            return urlResolver.GetVirtualPath(contentLink, null, new VirtualPathArguments { RouteValues = new RouteValueDictionary { { CategoryRoutingConstants.CurrentCategory, categoryLink } } }).GetUrl();
        }
    }
}