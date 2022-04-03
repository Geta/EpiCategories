using System.Web.Mvc;
using EpiCategories.SandboxApp.Models.Pages;
using EpiCategories.SandboxApp.Models.ViewModels;
using Geta.EpiCategories;
using System.Collections.Generic;
using System.Linq;

namespace EpiCategories.SandboxApp.Controllers
{   
    public class StandardPageController : PageControllerBase<StandardPage>
    {
        public ViewResult Index(StandardPage currentPage, IList<CategoryData> currentCategories)
        {
            var vm = PageViewModel.Create(currentPage);
            if (currentCategories != null && currentCategories.Any())
            {
                // Filter out content using categories
            }
            return View(vm);
        }
    }
}
