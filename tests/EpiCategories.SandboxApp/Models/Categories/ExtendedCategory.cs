using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EpiCategories.SandboxApp.Models.Categories
{
    [ContentType]
    public class ExtendedCategory : BasicCategory
    {
        [CultureSpecific] public virtual XhtmlString MainBody { get; set; }
    }
}
