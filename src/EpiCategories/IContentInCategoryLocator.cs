using System.Collections.Generic;
using System.Globalization;
using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface IContentInCategoryLocator
    {
        IEnumerable<T> GetDescendents<T>(ContentReference parentLink, IEnumerable<ContentReference> categories) where T : ICategoryContent;

        IEnumerable<T> GetDescendents<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategoryContent;

        IEnumerable<T> GetDescendents<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategoryContent;

        IEnumerable<T> GetChildren<T>(ContentReference parentLink, IEnumerable<ContentReference> categories) where T : ICategoryContent;

        IEnumerable<T> GetChildren<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategoryContent;

        IEnumerable<T> GetChildren<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategoryContent;
    }
}