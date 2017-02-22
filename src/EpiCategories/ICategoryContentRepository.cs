using System.Collections.Generic;
using System.Globalization;
using EPiServer.Core;

namespace Geta.EpiCategories
{
    public interface ICategoryContentRepository
    {
        ContentReference GetRootLink();
        T GetRoot<T>() where T : CategoryData;
        T Get<T>(ContentReference categoryLink) where T : CategoryData;
        bool TryGet<T>(ContentReference categoryLink, out T category) where T : CategoryData;
        T GetFirstBySegment<T>(string urlSegment, CultureInfo culture) where T : CategoryData;
        IEnumerable<T> List<T>() where T : CategoryData;
        IEnumerable<T> List<T>(ContentReference parentLink) where T : CategoryData;
    }
}