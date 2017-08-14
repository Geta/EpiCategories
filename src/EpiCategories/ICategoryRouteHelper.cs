using EPiServer.Core;
using EPiServer.Web.Routing;

namespace Geta.EpiCategories
{
    public interface ICategoryRouteHelper : IContentRouteHelper
    {
        ContentReference CategoryLink { get; }
        CategoryData Category { get; }
    }
}