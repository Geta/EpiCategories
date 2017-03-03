using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Web;
using Geta.EpiCategories.Extensions;

namespace Geta.EpiCategories
{
    public class DefaultCategoryContentLoader : ICategoryContentLoader
    {
        protected readonly IContentRepository ContentRepository;
        protected readonly LanguageResolver LanguageResolver;

        public DefaultCategoryContentLoader(IContentRepository contentRepository, LanguageResolver languageResolver)
        {
            ContentRepository = contentRepository;
            LanguageResolver = languageResolver;
        }

        public virtual T Get<T>(ContentReference categoryLink) where T : CategoryData
        {
            return ContentRepository.Get<T>(categoryLink, CreateDefaultLoadOptions());
        }

        public virtual T Get<T>(ContentReference categoryLink, CultureInfo culture) where T : CategoryData
        {
            return ContentRepository.Get<T>(categoryLink, culture);
        }

        public virtual T Get<T>(ContentReference categoryLink, LoaderOptions loaderOptions) where T : CategoryData
        {
            return ContentRepository.Get<T>(categoryLink, loaderOptions);
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference parentCategoryLink) where T : CategoryData
        {
            return GetChildren<T>(parentCategoryLink, CreateDefaultListOptions());
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference parentCategoryLink, CultureInfo culture) where T : CategoryData
        {
            return ContentRepository.GetChildren<T>(parentCategoryLink, culture);
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference parentCategoryLink, LoaderOptions loaderOptions) where T : CategoryData
        {
            return ContentRepository.GetChildren<T>(parentCategoryLink, loaderOptions);
        }

        public T GetFirstBySegment<T>(string urlSegment) where T : CategoryData
        {
            return GetFirstBySegment<T>(urlSegment, CreateDefaultLoadOptions());
        }

        public T GetFirstBySegment<T>(string urlSegment, CultureInfo culture) where T : CategoryData
        {
            var loaderOptions = new LoaderOptions
            {
                LanguageLoaderOption.Specific(culture)
            };

            return GetFirstBySegment<T>(urlSegment, loaderOptions);
        }

        public T GetFirstBySegment<T>(string urlSegment, LoaderOptions loaderOptions) where T : CategoryData
        {
            if (SiteDefinition.Current.SiteAssetsRoot != SiteDefinition.Current.GlobalAssetsRoot)
            {
                var firstSiteCategory = GetFirstBySegment<T>(ContentRepository.GetOrCreateSiteCategoriesRoot(), urlSegment, loaderOptions);

                if (firstSiteCategory != null)
                {
                    return firstSiteCategory;
                }
            }

            return GetFirstBySegment<T>(ContentRepository.GetOrCreateGlobalCategoriesRoot(), urlSegment, loaderOptions);
        }

        public virtual T GetFirstBySegment<T>(ContentReference parentLink, string urlSegment, LoaderOptions loaderOptions) where T : CategoryData
        {
            var descendents = ContentRepository.GetDescendents(parentLink);

            var categories = ContentRepository
                .GetItems(descendents, loaderOptions)
                .OfType<T>();

            return categories.FirstOrDefault(x => x.RouteSegment.Equals(urlSegment, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual IEnumerable<T> GetGlobalCategories<T>() where T : CategoryData
        {
            return GetChildren<T>(ContentRepository.GetOrCreateGlobalCategoriesRoot());
        }

        public virtual IEnumerable<T> GetGlobalCategories<T>(CultureInfo culture) where T : CategoryData
        {
            return GetChildren<T>(ContentRepository.GetOrCreateGlobalCategoriesRoot(), culture);
        }

        public virtual IEnumerable<T> GetGlobalCategories<T>(LoaderOptions loaderOptions) where T : CategoryData
        {
            return GetChildren<T>(ContentRepository.GetOrCreateGlobalCategoriesRoot(), loaderOptions);
        }

        public virtual IEnumerable<T> GetSiteCategories<T>() where T : CategoryData
        {
            return GetChildren<T>(ContentRepository.GetOrCreateSiteCategoriesRoot());
        }

        public virtual IEnumerable<T> GetSiteCategories<T>(CultureInfo culture) where T : CategoryData
        {
            return GetChildren<T>(ContentRepository.GetOrCreateSiteCategoriesRoot(), culture);
        }

        public virtual IEnumerable<T> GetSiteCategories<T>(LoaderOptions loaderOptions) where T : CategoryData
        {
            return GetChildren<T>(ContentRepository.GetOrCreateSiteCategoriesRoot(), loaderOptions);
        }

        public virtual bool TryGet<T>(ContentReference categoryLink, out T category) where T : CategoryData
        {
            return ContentRepository.TryGet(categoryLink, out category);
        }

        protected virtual LoaderOptions CreateDefaultLoadOptions()
        {
            return new LoaderOptions
            {
                LanguageLoaderOption.FallbackWithMaster(LanguageResolver.GetPreferredCulture())
            };
        }

        protected virtual LoaderOptions CreateDefaultListOptions()
        {
            return new LoaderOptions
            {
                LanguageLoaderOption.Fallback(LanguageResolver.GetPreferredCulture())
            };
        }
    }
}