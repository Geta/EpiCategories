using System.Collections.Generic;
using System.Globalization;
using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface IContentInCategoryLocator
    {
        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent;
    }
}