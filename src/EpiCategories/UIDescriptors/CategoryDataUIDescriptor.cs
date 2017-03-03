using EPiServer.Shell;

namespace Geta.EpiCategories.UIDescriptors
{
    [UIDescriptorRegistration]
    public class CategoryDataUIDescriptor : UIDescriptor<CategoryData>
    {
        public CategoryDataUIDescriptor() : base("epi-iconObjectCategory")
        {
            IsPrimaryType = true;
            ContainerTypes = new[]
            {
                typeof (CategoryData)
            };
        }
    }
}