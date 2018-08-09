using System;
using System.Globalization;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Web.Routing;

namespace Geta.EpiCategories.Routing
{
    public class CategoryDataValueProvider : IValueProvider
    {
        protected readonly ControllerContext ControllerContext;

        public CategoryDataValueProvider(ControllerContext controllerContext)
        {
            ControllerContext = controllerContext;
        }

        public bool ContainsPrefix(string prefix)
        {
            return prefix.Equals(CategoryRoutingConstants.CurrentCategory, StringComparison.InvariantCultureIgnoreCase);
        }

        public ValueProviderResult GetValue(string key)
        {
            if (ContainsPrefix(key) == false)
            {
                return null;
            }

            var categoryData = ControllerContext.RequestContext.GetCustomRouteData<CategoryData>(CategoryRoutingConstants.CurrentCategory);

            if (categoryData != null)
            {
                return new ValueProviderResult(categoryData, categoryData.ContentLink.ToString(), CultureInfo.InvariantCulture);
            }

            return null;
        }
    }
}