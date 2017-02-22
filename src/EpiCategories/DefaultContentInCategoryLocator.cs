using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using Geta.EpiCategories.Extensions;

namespace Geta.EpiCategories
{
    public class DefaultContentInCategoryLocator : IContentInCategoryLocator
    {
        protected readonly IContentLoader ContentLoader;
        protected readonly LanguageResolver LanguageResolver;

        public DefaultContentInCategoryLocator(IContentLoader contentLoader, LanguageResolver languageResolver)
        {
            ContentLoader = contentLoader;
            LanguageResolver = languageResolver;
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories) where T : ICategoryContent
        {
            return GetDescendents<T>(contentLink, categories, CreateDefaultListLoaderOptions());
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories, CultureInfo culture) where T : ICategoryContent
        {
            var loaderOptions = new LoaderOptions { LanguageLoaderOption.Specific(culture) };
            return GetDescendents<T>(contentLink, categories, loaderOptions);
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories, LoaderOptions loaderOptions) where T : ICategoryContent
        {
            var descendents = ContentLoader.GetDescendents(contentLink);

            return ContentLoader
                .GetItems(descendents, loaderOptions)
                .OfType<T>()
                .Where(x => x.Categories.ContainsAny(categories));
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference contentLink, ContentCategoryList categories) where T : ICategoryContent
        {
            return GetChildren<T>(contentLink, categories, CreateDefaultListLoaderOptions());
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference contentLink, ContentCategoryList categories, CultureInfo culture) where T : ICategoryContent
        {
            var loaderOptions = new LoaderOptions { LanguageLoaderOption.Specific(culture) };
            return GetChildren<T>(contentLink, categories, loaderOptions);
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference contentLink, ContentCategoryList categories, LoaderOptions loaderOptions) where T : ICategoryContent
        {
            return ContentLoader
                .GetChildren<T>(contentLink, loaderOptions)
                .Where(x => x.Categories.ContainsAny(categories));
        }

        protected virtual LoaderOptions CreateDefaultListLoaderOptions()
        {
            return new LoaderOptions
            {
                LanguageLoaderOption.Fallback(LanguageResolver.GetPreferredCulture())
            };
        }
    }
}