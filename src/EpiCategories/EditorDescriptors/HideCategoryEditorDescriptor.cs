using System;
using System.Collections.Generic;
using EPiServer.Cms.Shell.Extensions;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.EpiCategories.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(CategoryList))]
    public class HideCategoryEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            if (metadata.PropertyName == "icategorizable_category" && metadata.FindOwnerContent() is CategoryData)
            {
                metadata.ShowForEdit = false;
                metadata.ShowForDisplay = false;
            }
        }
    }
}