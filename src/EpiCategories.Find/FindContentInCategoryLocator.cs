using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Globalization;
using Geta.EpiCategories.Find.Extensions;

namespace Geta.EpiCategories.Find
{
    public class FindContentInCategoryLocator : DefaultContentInCategoryLocator
    {
        protected readonly IClient FindClient;

        public FindContentInCategoryLocator(IContentLoader contentLoader, LanguageResolver languageResolver, IClient findClient) : base(contentLoader, languageResolver)
        {
            FindClient = findClient;
        }

        public override IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories)
        {
            return GetDescendents<T>(contentLink, categories, (CultureInfo)null);
        }

        public override IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories, CultureInfo culture)
        {
            var search = FindClient.Search<T>()
                .Filter(x => x.Ancestors().Match(contentLink.ID.ToString()))
                .FilterHitsByCategories(categories);

            if (culture != null)
            {
                search = search.FilterOnLanguages(new[] {culture.Name});
            }

            return search.GetContentResult();
        }

        public override IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories, LoaderOptions loaderOptions)
        {
            throw new NotImplementedException("LoaderOptions parameter makes no sense for Episerver Find. Use overload GetDescendents<T>(ContentReference parentLink, ContentCategoryList categories, CultureInfo culture) instead.");
        }
    }
}