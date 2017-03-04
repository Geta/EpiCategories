using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Geta.EpiCategories.Extensions;
using Geta.EpiCategories.Routing;

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

            Global.RoutesRegistered += OnEpiserverRoutesRegistered;
            RouteTable.Routes.RegisterPartialRouter(new CategoryPartialRouter(context.Locate.ContentLoader(), context.Locate.Advanced.GetInstance<ICategoryContentLoader>()));
        }

        private void OnEpiserverRoutesRegistered(object sender, RouteRegistrationEventArgs routeRegistrationEventArgs)
        {
            RouteTable.Routes.MapCategoryRoute("sharedcategories", "{language}/{node}/{partial}/{action}", new {action = "index"}, sd => sd.GlobalAssetsRoot);
            RouteTable.Routes.MapCategoryRoute("sitecategories", "{language}/{node}/{partial}/{action}", new { action = "index" }, sd => sd.SiteAssetsRoot);
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            ConfigureContainer(context.Services);
        }

        private static void ConfigureContainer(IServiceConfigurationProvider services)
        {
            services.AddSingleton<ICategoryContentLoader, DefaultCategoryContentLoader>();
            services.AddSingleton<IContentInCategoryLocator, DefaultContentInCategoryLocator>();
        }

        private void OnCreatingContent(object sender, ContentEventArgs args)
        {
            var categoryData = args.Content as CategoryData;

            if (categoryData != null && string.IsNullOrWhiteSpace(categoryData.RouteSegment))
            {
                categoryData.RouteSegment = ServiceLocator.Current.GetInstance<IUrlSegmentCreator>().Create(categoryData);
            }
        }
    }
}