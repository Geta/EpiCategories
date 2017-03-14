using System.Collections.Generic;
using System.Web.Routing;
using EPiServer.Core;
using EPiServer.Web.Routing;
using Geta.EpiCategories.Routing;

namespace Geta.EpiCategories.Extensions
{
    public static class UrlResolverExtensions
    {
        public static string GetCategoryRoutedUrl(this UrlResolver urlResolver, ContentReference contentLink, ContentReference categoryContentLink)
        {
            return urlResolver.GetVirtualPath(contentLink, null,
                new VirtualPathArguments
                {
                    RouteValues = new RouteValueDictionary {{CategoryRoutingConstants.CurrentCategory, categoryContentLink}}
                })
                .GetUrl();
        }

        public static string GetCategoryRoutedUrl(this UrlResolver urlResolver, ContentReference contentLink, IEnumerable<ContentReference> categoryContentLinks)
        {
            return urlResolver.GetVirtualPath(contentLink, null,
                new VirtualPathArguments
                {
                    RouteValues = new RouteValueDictionary { { CategoryRoutingConstants.CurrentCategories, categoryContentLinks } }
                })
                .GetUrl();
        }
    }
}