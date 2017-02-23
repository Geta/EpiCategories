using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface ICategoryContent : IContentData
    {
         ContentCategoryList Categories { get; set; } 
    }
}