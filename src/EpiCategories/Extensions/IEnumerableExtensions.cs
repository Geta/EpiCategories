using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;

namespace Geta.EpiCategories.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool ContainsAny(this IEnumerable<ContentReference> contentLinks, IEnumerable<ContentReference> otherContentLinks)
        {
            if (contentLinks == null)
            {
                return false;
            }

            return contentLinks.Any(x => otherContentLinks.Any(y => y.CompareToIgnoreWorkID(x)));
        }
    }
}