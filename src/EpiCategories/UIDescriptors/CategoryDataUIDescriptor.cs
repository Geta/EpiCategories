using EPiServer.Shell;

namespace Geta.EpiCategories.UIDescriptors
{
    [UIDescriptorRegistration]
    public class CategoryDataUIDescriptor : UIDescriptor<CategoryData>
    {
        public CategoryDataUIDescriptor() : base("epi-iconCategory")
        {
            this.IsPrimaryType = true;
            this.ContainerTypes = new[]
            {
                typeof (CategoryData)
            };


        }
    }
}