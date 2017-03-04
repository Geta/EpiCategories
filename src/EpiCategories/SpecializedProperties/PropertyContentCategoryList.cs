using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.PlugIn;
using EPiServer.SpecializedProperties;

namespace Geta.EpiCategories.SpecializedProperties
{
    [PropertyDefinitionTypePlugIn]
    [Serializable]
    public class PropertyContentCategoryList : PropertyContentReferenceList
    {
        public override IList<ContentReference> List
        {
            get
            {
                return base.List ?? new List<ContentReference>();
            }
            set
            {
                base.List = value;
            }
        }
    }
}