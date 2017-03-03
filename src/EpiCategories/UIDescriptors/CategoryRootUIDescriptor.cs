using EPiServer.Shell;

namespace Geta.EpiCategories.UIDescriptors
{
    [UIDescriptorRegistration]
    public class CategoryRootUIDescriptor : UIDescriptor<CategoryRoot>, IEditorDropBehavior
    {
        public CategoryRootUIDescriptor() : base("epi-iconObjectFolder")
        {
            DefaultView = "formedit";
            EditorDropBehaviour = EditorDropBehavior.CreateLink;
            ContainerTypes = new[] { typeof(CategoryData) };
        }

        public EditorDropBehavior EditorDropBehaviour { get; set; }
    }
}