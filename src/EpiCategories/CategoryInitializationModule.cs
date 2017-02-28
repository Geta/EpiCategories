using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Enterprise;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Geta.EpiCategories.Routing;
using Geta.EpiCategories.Transfer;

namespace Geta.EpiCategories
{
    [InitializableModule]
    [ModuleDependency(typeof(DataInitialization))]
    public class CategoryInitializationModule : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var contentEvents = context.Locate.ContentEvents();
            contentEvents.CreatingContent += OnCreatingContent;

            RegisterImportEvents(context.Locate.Advanced);
            RegisterExportEvents(context.Locate.Advanced);

            RouteTable.Routes.RegisterPartialRouter(new CategoryPartialRouter(context.Locate.ContentLoader(), context.Locate.Advanced.GetInstance<ICategoryContentLoader>()));
        }

        public void Uninitialize(InitializationEngine context)
        {
            var factory = ServiceLocator.Current;
            UnregisterImportEvents(factory);
            UnregisterExportEvents(factory);
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            ConfigureContainer(context.Services);
        }

        private void RegisterImportEvents(IServiceLocator factory)
        {
            var importEvents = factory.GetInstance<IDataImportEvents>();
            var propertyContentCategoryListTransform = factory.GetInstance<PropertyContentCategoryListTransform>();

            importEvents.PropertyImporting += propertyContentCategoryListTransform.ImportEventHandler;
            importEvents.Starting += propertyContentCategoryListTransform.StartingEventHandler;
            importEvents.Completed += propertyContentCategoryListTransform.CompletedEventHandler;
        }

        private void RegisterExportEvents(IServiceLocator factory)
        {
            var exportEvents = factory.GetInstance<IDataExportEvents>();
            var propertyContentCategoryListTransform = factory.GetInstance<PropertyContentCategoryListTransform>();

            exportEvents.PropertyExporting += propertyContentCategoryListTransform.ExportEventHandler;
        }

        private void UnregisterImportEvents(IServiceLocator factory)
        {
            var importEvents = factory.GetInstance<IDataImportEvents>();
            var propertyContentCategoryListTransform = factory.GetInstance<PropertyContentCategoryListTransform>();

            importEvents.PropertyImporting -= propertyContentCategoryListTransform.ImportEventHandler;
            importEvents.Starting -= propertyContentCategoryListTransform.StartingEventHandler;
            importEvents.Completed -= propertyContentCategoryListTransform.CompletedEventHandler;
        }

        private void UnregisterExportEvents(IServiceLocator factory)
        {
            var exportEvents = factory.GetInstance<IDataExportEvents>();
            var propertyContentCategoryListTransform = factory.GetInstance<PropertyContentCategoryListTransform>();

            exportEvents.PropertyExporting -= propertyContentCategoryListTransform.ExportEventHandler;
        }

        private static void ConfigureContainer(IServiceConfigurationProvider services)
        {
            services.AddSingleton<ICategoryContentLoader, DefaultCategoryContentLoader>();
            services.AddSingleton<IContentInCategoryLocator, DefaultContentInCategoryLocator>();
        }

        private void OnCreatingContent(object sender, ContentEventArgs args)
        {
            var categoryData = args.Content as CategoryData;

            if (categoryData != null)
            {
                categoryData.RouteSegment = ServiceLocator.Current.GetInstance<IUrlSegmentCreator>().Create(categoryData);
            }
        }
    }
}