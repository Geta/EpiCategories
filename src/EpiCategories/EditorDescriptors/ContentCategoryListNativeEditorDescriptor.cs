using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.EpiCategories.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentCategoryList), UIHint = CategoryUIHint.ContentReferenceList)]
    public class ContentCategoryListNativeEditorDescriptor : ContentReferenceListEditorDescriptor
    {
        private readonly IEnumerable<IContentRepositoryDescriptor> _contentRepositoryDescriptors;

        public ContentCategoryListNativeEditorDescriptor(IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors, IContentLoader contentLoader) : base(contentRepositoryDescriptors, contentLoader)
        {
            _contentRepositoryDescriptors = contentRepositoryDescriptors;
            AllowedTypes = new[] {typeof (CategoryData)};
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            var categoryRepositoryDescriptor = _contentRepositoryDescriptors.FirstOrDefault(x => x.Key == CategoryContentRepositoryDescriptor.RepositoryKey);

            if (categoryRepositoryDescriptor == null)
                return;

            metadata.EditorConfiguration["roots"] = categoryRepositoryDescriptor.Roots;
        }
    }
}