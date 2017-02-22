using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.PlugIn;

namespace Geta.EpiCategories.SpecializedProperties
{
    [PropertyDefinitionTypePlugIn]
    [Serializable]
    public class PropertyContentCategoryList : PropertyList<ContentReference>
    {
        public override IList<ContentReference> List
        {
            get
            {
                if (base.List != null)
                {
                    return new ContentCategoryList(base.List);
                }

                return base.List;
            }
            set
            {
                base.List = value != null ? new ContentCategoryList(value) : null;
            }
        }

        public override PropertyData ParseToObject(string value)
        {
            PropertyContentCategoryList propertyData = new PropertyContentCategoryList();
            propertyData.ParseToSelf(value);
            propertyData.IsModified = false;
            return propertyData;
        }

        protected override ContentReference ParseItem(string value)
        {
            if (string.IsNullOrEmpty(value) == false)
                return ContentReference.Parse(value);

            return ContentReference.EmptyReference;
        }

        public override Type PropertyValueType { get { return typeof (ContentCategoryList); } }
    }
}