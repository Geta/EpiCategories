using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.EpiCategories.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentCategoryList))]
    public class ContentCategoryListEditorDescriptor : ContentReferenceListEditorDescriptor
    {
        public ContentCategoryListEditorDescriptor(IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors, IContentLoader contentLoader) : base(contentRepositoryDescriptors, contentLoader)
        {
            AllowedTypes = new[] { typeof(CategoryData) };
        }
    }
}