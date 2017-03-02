using EPiServer.Shell;

namespace Geta.EpiCategories.UIDescriptors
{
    [UIDescriptorRegistration]
    public class CategoryDataUIDescriptor : UIDescriptor<CategoryData>
    {
        public CategoryDataUIDescriptor() : base("epi-iconCategory")
        {
            IsPrimaryType = true;
            ContainerTypes = new[]
            {
                typeof (CategoryData)
            };
        }
    }
}