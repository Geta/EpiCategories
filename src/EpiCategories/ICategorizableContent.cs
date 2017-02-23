using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface ICategorizableContent
    {
         ContentCategoryList Categories { get; set; } 
    }
}