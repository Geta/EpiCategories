using EPiServer.Core;

namespace EpiCategories.SandboxApp.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
