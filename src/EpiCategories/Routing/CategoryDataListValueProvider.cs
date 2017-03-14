using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Web.Routing;

namespace Geta.EpiCategories.Routing
{
    public class CategoryDataListValueProvider : IValueProvider
    {
        protected readonly ControllerContext ControllerContext;

        public CategoryDataListValueProvider(ControllerContext controllerContext)
        {
            ControllerContext = controllerContext;
        }

        public bool ContainsPrefix(string prefix)
        {
            return prefix.Equals(CategoryRoutingConstants.CurrentCategories, StringComparison.InvariantCultureIgnoreCase);
        }

        public ValueProviderResult GetValue(string key)
        {
            if (ContainsPrefix(key) == false)
            {
                return null;
            }

            var categories = ControllerContext.RequestContext.GetCustomRouteData<IList<CategoryData>>(CategoryRoutingConstants.CurrentCategories);

            if (categories != null)
            {
                return new ValueProviderResult(categories, string.Join(CategoryPartialRouter.CategorySeparator, categories.Select(x => x.ContentLink)), CultureInfo.InvariantCulture);
            }

            return new ValueProviderResult(new List<CategoryData>(), string.Empty, CultureInfo.InvariantCulture);
        }
    }
}