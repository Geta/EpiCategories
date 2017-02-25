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

            RegisterImportEvents(context.Locate.Advanced.GetInstance<IDataImportEvents>(), context.Locate.Advanced);
            RegisterExportEvents(context.Locate.Advanced.GetInstance<IDataExportEvents>(), context.Locate.Advanced);

            RouteTable.Routes.RegisterPartialRouter(new CategoryPartialRouter(context.Locate.ContentLoader(), context.Locate.Advanced.GetInstance<ICategoryContentRepository>()));
        }

        private void RegisterImportEvents(IDataImportEvents importEvents, IServiceLocator factory)
        {
            var propertyContentCategoryListTransform = factory.GetInstance<PropertyContentCategoryListTransform>();
            importEvents.PropertyImporting += propertyContentCategoryListTransform.ImportEventHandler;
            importEvents.Starting += propertyContentCategoryListTransform.StartingEventHandler;
            importEvents.Completed += propertyContentCategoryListTransform.CompletedEventHandler;
        }

        private void RegisterExportEvents(IDataExportEvents exportEvents, IServiceLocator factory)
        {
            exportEvents.PropertyExporting += factory.GetInstance<PropertyContentCategoryListTransform>().ExportEventHandler;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var factory = ServiceLocator.Current;
            var importEvents = factory.GetInstance<IDataImportEvents>();
            var propertyContentCategoryListTransform = factory.GetInstance<PropertyContentCategoryListTransform>();

            factory.GetInstance<IContentEvents>().CreatingContent -= OnCreatingContent;
            importEvents.PropertyImporting -= propertyContentCategoryListTransform.ImportEventHandler;
            importEvents.Completed -= propertyContentCategoryListTransform.CompletedEventHandler;
            factory.GetInstance<IDataExportEvents>().PropertyExporting -= propertyContentCategoryListTransform.ExportEventHandler;
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            ConfigureContainer(context.Services);
        }

        private static void ConfigureContainer(IServiceConfigurationProvider services)
        {
            services.AddSingleton<ICategoryContentRepository, DefaultCategoryContentRepository>();
            services.AddSingleton<IContentInCategoryLocator, DefaultContentInCategoryLocator>();
        }

        private void OnCreatingContent(object sender, ContentEventArgs args)
        {
            var categoryData = args.Content as CategoryData;

            if (categoryData != null)
            {
                categoryData.URLSegment = ServiceLocator.Current.GetInstance<IUrlSegmentCreator>().Create(categoryData);
            }
        }
    }
}