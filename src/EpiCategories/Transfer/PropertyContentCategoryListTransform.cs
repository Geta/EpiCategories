using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Core.Transfer.Internal;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Enterprise;
using EPiServer.Enterprise.Transfer;
using EPiServer.Framework;
using EPiServer.Framework.Serialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Geta.EpiCategories.SpecializedProperties;

namespace Geta.EpiCategories.Transfer
{
    [ServiceConfiguration(IncludeServiceAccessor = false)]
    public class PropertyContentCategoryListTransform
    {
        private readonly IPermanentLinkMapper _permanentLinkMapper;
        private readonly IContentRepository _contentRepository;
        private readonly ServiceAccessor<IDependentContentTransfer> _dependentContentTransferAccessor;
        private readonly IObjectSerializer _objectSerializer;

        public PropertyContentCategoryListTransform(IPermanentLinkMapper permanentLinkMapper, IContentRepository contentRepository, ServiceAccessor<IDependentContentTransfer> dependentContentTransferAccessor, IObjectSerializer objectSerializer)
        {
            _permanentLinkMapper = permanentLinkMapper;
            _contentRepository = contentRepository;
            _dependentContentTransferAccessor = dependentContentTransferAccessor;
            _objectSerializer = objectSerializer;
        }

        public void ImportEventHandler(ITransferContext transfercontext, TransformPropertyEventArgs e)
        {
            var contentTransferContext = transfercontext as IContentTransferContext;

            if (contentTransferContext == null || e == null || e.PropertySource == null || e.PropertySource.Type != PropertyDataType.LongString)
                return;

            RawProperty propertySource = e.PropertySource;

            if (propertySource.TypeName != typeof(PropertyContentCategoryList).FullName)
            {
                return;
            }

            propertySource.Type = PropertyDataType.Json;
            PropertyContentCategoryList property = e.PropertyDestination as PropertyContentCategoryList;

            if (property == null)
            {
                return;
            }

            var exportableLinks = _objectSerializer.Deserialize<IList<string>>(propertySource.Value);
            var referencedGuids = new List<Guid>();

            for (int i = exportableLinks.Count - 1; i >= 0; i--)
            {
                var exportableLink = ExportableLink.Find(exportableLinks[i]);

                if (exportableLink.Count > 0)
                {
                    Guid contentGuid = exportableLink[0].ContentGuid;
                    Guid guid;

                    if (contentTransferContext.LinkGuidMap.TryGetValue(contentGuid, out guid))
                    {
                        if (guid == Guid.Empty)
                            contentTransferContext.LinkGuidMap[contentGuid] = guid = Guid.NewGuid();
                    }
                    else
                    {
                        guid = contentGuid;
                    }

                    referencedGuids.Add(guid);
                }
            }

            AddOnCompletedTask(e.ContextContent, e.PropertySource.Name, referencedGuids);
            property.Value = null;
            e.IsHandled = true;
        }

        public void ExportEventHandler(ITransferContext transferContext, TransformPropertyEventArgs e)
        {
            IContentTransferContext contentTransferContext = transferContext;

            if (contentTransferContext == null || e == null || e.PropertySource == null || e.PropertySource.Type != PropertyDataType.Json)
                return;
            
            if (e.PropertySource.TypeName != typeof(PropertyContentCategoryList).FullName)
            {
                return;
            }

            var contentLinks = _objectSerializer.Deserialize<IList<ContentReference>>(e.PropertySource.Value);

            if (contentLinks != null)
            {
                var exportableLinks = new List<string>();

                foreach (var contentLink in contentLinks)
                {
                    PermanentLinkMap permanentLinkMap = _permanentLinkMapper.Find(contentLink);

                    if (permanentLinkMap != null)
                    {
                        var exportableLink = transferContext.ForceNewIdentitiesOnExport
                            ? ExportableLink.Create(permanentLinkMap.Guid, string.Empty, string.Empty, transferContext, true)
                            : ExportableLink.Create(permanentLinkMap.Guid, string.Empty, string.Empty, transferContext, false);

                        exportableLinks.Add(exportableLink.ToString());
                    }
                }

                if (MetaDataProperties.GetInterfaceForPageDataMetaDataProperty(e.PropertySource.Name) == null)
                    ExportDependentContent(contentLinks, transferContext);

                e.PropertySource.Type = PropertyDataType.LongString;
                e.PropertySource.Value = _objectSerializer.Serialize(exportableLinks);
            }

            e.IsHandled = true;
        }

        private void AddOnCompletedTask(RawContent content, string propertyName, List<Guid> referencedContentGuids)
        {
            var guidProperty = content.Property.FirstOrDefault(x => x.Name.Equals("PageGUID", StringComparison.InvariantCultureIgnoreCase));

            if (guidProperty == null)
                return;

            var tasks = ContextCache.Current["ImportCompletedTasks"] as IList<CompletedTask>;

            tasks.Add(new CompletedTask
            {
                ContentGUID = Guid.Parse(guidProperty.Value),
                PropertyName = propertyName,
                ReferencedContentGuids = referencedContentGuids
            });
        }

        private void ExportDependentContent(IEnumerable<ContentReference> contentLinks, IContentTransferContext transferContext)
        {
            IDependentContentTransfer dependentContentTransfer = _dependentContentTransferAccessor();
            dependentContentTransfer.TransferContext = transferContext;

            foreach (ContentReference contentLink in contentLinks)
                dependentContentTransfer.ExportDependentContent(_contentRepository.Get<IContent>(contentLink));
        }

        public void CompletedEventHandler(ITransferContext transfercontext, DataImporterContextEventArgs e)
        {
            var tasks = ContextCache.Current["ImportCompletedTasks"] as IList<CompletedTask>;

            foreach (var task in tasks)
            {
                var contentGuid = task.ContentGUID;
                ContentData content;

                if (_contentRepository.TryGet(contentGuid, out content) == false)
                {
                    continue;
                }

                content = content.CreateWritableClone() as ContentData;
                var property = content.Property[task.PropertyName] as PropertyContentCategoryList ?? new PropertyContentCategoryList();
                var contentLinks = new List<ContentReference>();

                foreach (var guid in task.ReferencedContentGuids)
                {
                    var linkMap = _permanentLinkMapper.Find(guid);

                    if (linkMap == null)
                    {
                        continue;
                    }

                    contentLinks.Add(linkMap.ContentReference);
                }

                if (contentLinks.Count > 0)
                {
                    property.Value = contentLinks;
                    _contentRepository.Save((IContent)content, SaveAction.Publish, AccessLevel.NoAccess);
                }
            }

            ContextCache.Current["ImportCompletedTasks"] = null;
        }

        public void StartingEventHandler(ITransferContext transfercontext, DataImporterContextEventArgs e)
        {
            ContextCache.Current["ImportCompletedTasks"] = new List<CompletedTask>();
        }
    }

    public class CompletedTask
    {
        public Guid ContentGUID { get; set; }
        public string PropertyName { get; set; }
        public List<Guid> ReferencedContentGuids { get; set; }
    }
}