using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace Geta.EpiCategories
{
    [ServiceConfiguration(typeof(ICategoryContentRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class DefaultCategoryContentRepository : ICategoryContentRepository
    {
        protected readonly IContentRepository ContentRepository;

        public DefaultCategoryContentRepository(IContentRepository contentRepository)
        {
            ContentRepository = contentRepository;
        }

        public virtual ContentReference GetRootLink()
        {
            var firstCategoryData = ContentRepository
                .GetChildren<CategoryData>(ContentReference.RootPage, new LoaderOptions { new LanguageLoaderOption { FallbackBehaviour = LanguageBehaviour.FallbackWithMaster } })
                .FirstOrDefault();

            if (firstCategoryData != null)
            {
                return firstCategoryData.ContentLink.ToReferenceWithoutVersion();
            }

            firstCategoryData = ContentRepository.GetDefault<CategoryData>(ContentReference.RootPage);
            firstCategoryData.Name = "Categories";
            firstCategoryData.IsSelectable = false;

            var rootLink = ContentRepository.Save(firstCategoryData, SaveAction.Publish, AccessLevel.NoAccess);
            return rootLink.ToReferenceWithoutVersion();
        }

        public virtual T GetRoot<T>() where T : CategoryData
        {
            return Get<T>(GetRootLink());
        }

        public virtual T Get<T>(ContentReference categoryLink) where T : CategoryData
        {
            return ContentRepository.Get<T>(categoryLink);
        }

        public virtual bool TryGet<T>(ContentReference categoryLink, out T category) where T : CategoryData
        {
            return ContentRepository.TryGet(categoryLink, out category);
        }

        public T GetFirstBySegment<T>(string urlSegment, CultureInfo culture) where T : CategoryData
        {
            var descendents = ContentRepository.GetDescendents(GetRootLink());
            var categories = ContentRepository
                .GetItems(descendents, new LoaderOptions { new LanguageLoaderOption { FallbackBehaviour = LanguageBehaviour.FallbackWithMaster } })
                .OfType<T>();

            return categories
                .FirstOrDefault(x => x.URLSegment.Equals(urlSegment, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual IEnumerable<T> List<T>() where T : CategoryData
        {
            return List<T>(GetRootLink());
        }

        public virtual IEnumerable<T> List<T>(ContentReference parentLink) where T : CategoryData
        {
            return ContentRepository.GetChildren<T>(parentLink);
        }
    }
}