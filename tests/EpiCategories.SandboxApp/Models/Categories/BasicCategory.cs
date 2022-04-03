using EPiServer.DataAnnotations;
using Geta.EpiCategories;

namespace EpiCategories.SandboxApp.Models.Categories
{
    [ContentType]
    public class BasicCategory : CategoryData
    {
        public virtual bool IsOverview { get; set; }
    }
}
