using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;

namespace Geta.EpiCategories.Find.Extensions
{
    public static class ITypeSearchExtensions
    {
        public static ITypeSearch<T> FilterByCategories<T>(this ITypeSearch<T> search, IEnumerable<ContentReference> categories) where T : ICategorizableContent
        {
            if (categories == null || categories.Any() == false)
            {
                return search;
            }

            return search.Filter(x => x.Categories().In(categories));
        }

        public static ITypeSearch<T> FilterHitsByCategories<T>(this ITypeSearch<T> search, IEnumerable<ContentReference> categories) where T : ICategorizableContent
        {
            if (categories == null || categories.Any() == false)
            {
                return search;
            }

            return search.FilterHits(x => x.Categories().In(categories));
        }

        public static ITypeSearch<T> ContentCategoriesFacet<T>(this ITypeSearch<T> request) where T : ICategorizableContent
        {
            return request.ContentReferenceFacet(x => x.Categories());
        }

        public static ITypeSearch<T> ContentReferenceFacet<T>(this ITypeSearch<T> request, Expression<Func<T, IEnumerable<string>>> fieldExpression) where T : ICategorizableContent
        {
            return request.TermsFacetFor(fieldExpression);
        }
    }
}