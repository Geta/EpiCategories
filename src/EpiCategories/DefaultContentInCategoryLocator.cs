using System;
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

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference parentLink, IEnumerable<ContentReference> categories) where T : ICategoryContent
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategoryContent
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategoryContent
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference parentLink, IEnumerable<ContentReference> categories) where T : ICategoryContent
        {
            return GetChildren<T>(parentLink, categories, CreateDefaultListLoaderOptions());
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategoryContent
        {
            var loaderOptions = new LoaderOptions { LanguageLoaderOption.Specific(culture) };
            return GetChildren<T>(parentLink, categories, loaderOptions);
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference parentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategoryContent
        {
            return ContentLoader.GetChildren<T>(parentLink, loaderOptions).Where(x => x.Categories.ContainsAny(categories));
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