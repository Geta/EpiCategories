using EPiServer.Core;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.EpiCategories.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentReference), UIHint = "Category")]
    public class CategorySelectionEditorDescriptor : ContentReferenceEditorDescriptor<CategoryData>
    {
        public override string RepositoryKey => CategoryContentRepositoryDescriptor.RepositoryKey;
    }
}