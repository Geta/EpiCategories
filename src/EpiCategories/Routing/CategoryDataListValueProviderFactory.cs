using System.Web.Mvc;

namespace Geta.EpiCategories.Routing
{
    public class CategoryDataListValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new CategoryDataListValueProvider(controllerContext);
        }
    }
}