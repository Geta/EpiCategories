using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.EpiCategories.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentCategoryList))]
    public class ContentCategoryListEditorDescriptor : EditorDescriptor
    {
        private readonly IEnumerable<IContentRepositoryDescriptor> _contentRepositoryDescriptors;
        private readonly CategorySettings _categorySettings;

        public ContentCategoryListEditorDescriptor(IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors, CategorySettings categorySettings)
        {
            _contentRepositoryDescriptors = contentRepositoryDescriptors;
            _categorySettings = categorySettings;
            ClientEditingClass = "geta-epicategories/widget/CategorySelector";
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            if (AllowedTypes == null)
            {
                AllowedTypes = new[] { typeof(CategoryData) };
            }

            base.ModifyMetadata(metadata, attributes);
        }

        protected override void SetEditorConfiguration(ExtendedMetadata metadata)
        {
            base.SetEditorConfiguration(metadata);

            metadata.EditorConfiguration["categorySettings"] = _categorySettings;
            metadata.EditorConfiguration["repositoryKey"] = CategoryContentRepositoryDescriptor.RepositoryKey;

            var categoryRepositoryDescriptor = _contentRepositoryDescriptors.FirstOrDefault(x => x.Key == CategoryContentRepositoryDescriptor.RepositoryKey);

            if (categoryRepositoryDescriptor == null)
                return;

            metadata.EditorConfiguration["roots"] = categoryRepositoryDescriptor.Roots;
        }
    }
}