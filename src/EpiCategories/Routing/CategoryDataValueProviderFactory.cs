using System.Web.Mvc;

namespace Geta.EpiCategories.Routing
{
    public class CategoryDataValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new CategoryDataValueProvider(controllerContext);
        }
    }
}