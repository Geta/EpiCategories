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
        protected readonly IContentRepository ContentRepository;
        protected readonly LanguageResolver LanguageResolver;

        public DefaultContentInCategoryLocator(IContentRepository contentRepository, LanguageResolver languageResolver)
        {
            ContentRepository = contentRepository;
            LanguageResolver = languageResolver;
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories) where T : ICategorizableContent, IContent
        {
            return GetDescendents<T>(contentLink, categories, CreateDefaultListLoaderOptions());
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategorizableContent, IContent
        {
            var loaderOptions = new LoaderOptions { LanguageLoaderOption.Specific(culture) };
            return GetDescendents<T>(contentLink, categories, loaderOptions);
        }

        public virtual IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent
        {
            if (categories != null && categories.Any())
            {
                var referenceContentLinks = new List<ContentReference>();

                foreach (var category in categories)
                {
                    var referencesToContent = ContentRepository.GetReferencesToContent(category, false);
                    referenceContentLinks.AddRange(referencesToContent.Select(x => x.OwnerID));
                }

                return ContentRepository
                    .GetItems(referenceContentLinks.Distinct(), loaderOptions)
                    .OfType<T>();
            }

            var contentLinks = ContentRepository.GetDescendents(contentLink);

            return ContentRepository
                .GetItems(contentLinks, loaderOptions)
                .OfType<T>();
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories) where T : ICategorizableContent, IContent
        {
            return GetChildren<T>(contentLink, categories, CreateDefaultListLoaderOptions());
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategorizableContent, IContent
        {
            var loaderOptions = new LoaderOptions { LanguageLoaderOption.Specific(culture) };
            return GetChildren<T>(contentLink, categories, loaderOptions);
        }

        public virtual IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent
        {
            return ContentRepository
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