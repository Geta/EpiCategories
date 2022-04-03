using System.Web.Mvc;
using EpiCategories.SandboxApp.Models.Categories;
using EPiServer.Core;
using EPiServer.Web.Mvc;

namespace EpiCategories.SandboxApp.Controllers
{
    public class BasicCategoryController : ContentController<BasicCategory>
    {
        public ActionResult Index(BasicCategory currentContent, int page = 1)
        {
            if (currentContent.IsOverview)
            {
                return View(currentContent);
            }

            return View(currentContent);
        }
    }
}
