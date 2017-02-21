using System.Collections.Generic;
using EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.EpiCategories.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(IList<ContentReference>), UIHint = "CategoryList")]
    public class CategoryListEditorDescriptor : ContentReferenceListEditorDescriptor
    {
        public CategoryListEditorDescriptor(IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors, IContentLoader contentLoader) : base(contentRepositoryDescriptors, contentLoader)
        {
            AllowedTypes = new[] {typeof (CategoryData)};
        }
    }
}