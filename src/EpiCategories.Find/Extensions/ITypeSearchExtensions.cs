using EPiServer.Find;

namespace Geta.EpiCategories.Find.Extensions
{
    public static class ITypeSearchExtensions
    {
        public static ITypeSearch<T> FilterByCategories<T>(this ITypeSearch<T> search, ContentCategoryList categories) where T : ICategorizableContent
        {
            return search.Filter(x => x.Categories.In(categories));
        }

        public static ITypeSearch<T> FilterHitsByCategories<T>(this ITypeSearch<T> search, ContentCategoryList categories) where T : ICategorizableContent
        {
            return search.FilterHits(x => x.Categories.In(categories));
        }
    }
}