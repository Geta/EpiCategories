using System;
using EPiServer.Core;

namespace Geta.EpiCategories.Transfer
{
    public class UnresolvedContentReference : ContentReference
    {
        public readonly Guid ContentGUID;

        public UnresolvedContentReference(Guid contentGuid)
        {
            ContentGUID = contentGuid;
        }
    }
}