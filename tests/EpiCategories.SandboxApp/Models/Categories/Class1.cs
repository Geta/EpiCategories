using EPiServer.Core;
using EPiServer.DataAnnotations;
using Geta.EpiCategories;

namespace EpiCategories.SandboxApp.Models.Categories
{
    [ContentType]
    public class BasicCategory : CategoryData { }

    [ContentType]
    public class ExtendedCategory : BasicCategory
    {
        [CultureSpecific] public virtual XhtmlString MainBody { get; set; }
    }
}
