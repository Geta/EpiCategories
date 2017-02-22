using System.Collections.Generic;
using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface ICategoryContent : IContentData
    {
         IEnumerable<ContentReference> Categories { get; set; } 
    }
}