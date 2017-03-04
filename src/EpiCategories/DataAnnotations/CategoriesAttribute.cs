using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;

namespace Geta.EpiCategories.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CategoriesAttribute : Attribute, IMetadataAware
    {
        private readonly CategorySettings _categorySettings;
        private readonly IEnumerable<IContentRepositoryDescriptor> _contentRepositoryDescriptors;

        public CategoriesAttribute() : this(ServiceLocator.Current.GetInstance<IEnumerable<IContentRepositoryDescriptor>>(), ServiceLocator.Current.GetInstance<CategorySettings>())
        {
        }

        public CategoriesAttribute(IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors, CategorySettings categorySettings)
        {
            _contentRepositoryDescriptors = contentRepositoryDescriptors;
            _categorySettings = categorySettings;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var extendedMetadata = metadata as ExtendedMetadata;

            if (extendedMetadata == null)
            {
                return;
            }

            var allowedTypes = new[] {typeof (CategoryData)};
            var categoryRepositoryDescriptor = _contentRepositoryDescriptors.First(x => x.Key == CategoryContentRepositoryDescriptor.RepositoryKey);
            extendedMetadata.ClientEditingClass = "geta-epicategories/widget/CategorySelector";
            extendedMetadata.EditorConfiguration["AllowedTypes"] = allowedTypes;
            extendedMetadata.EditorConfiguration["AllowedDndTypes"] = allowedTypes;
            extendedMetadata.OverlayConfiguration["AllowedDndTypes"] = allowedTypes;
            extendedMetadata.EditorConfiguration["categorySettings"] = _categorySettings;
            extendedMetadata.EditorConfiguration["repositoryKey"] = CategoryContentRepositoryDescriptor.RepositoryKey;
            extendedMetadata.EditorConfiguration["settings"] = categoryRepositoryDescriptor;
            extendedMetadata.EditorConfiguration["roots"] = categoryRepositoryDescriptor.Roots;
        }
    }
}