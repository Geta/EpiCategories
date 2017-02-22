using System.Collections.Generic;
using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface ICategoryContent : IContent
    {
         ContentCategoryList Categories { get; set; } 
    }
}