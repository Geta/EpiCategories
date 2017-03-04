using System.Collections.Generic;
using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface ICategorizableContent
    {
         IList<ContentReference> Categories { get; set; } 
    }
}