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
using System.Web.Mvc;
using System.Web.Routing;

namespace Geta.EpiCategories
{
    [InitializableModule]
    [ModuleDependency(typeof(DataInitialization))]
    public class CategoryInitializationModule : IConfigurableModule
    {
        private bool _isInitialized;

        public void Initialize(InitializationEngine context)
        {
            if (_isInitialized)
            {
                return;
            }

            ValueProviderFactories.Factories.Add(new CategoryDataValueProviderFactory());
            ValueProviderFactories.Factories.Add(new CategoryDataListValueProviderFactory());

            var locator = context.Locate.Advanced;
            var contentEvents = context.Locate.ContentEvents();

            contentEvents.CreatingContent += OnCreatingContent;
            Global.RoutesRegistered += OnEpiserverRoutesRegistered;
            RouteTable.Routes.RegisterPartialRouter(locator.GetInstance<CategoryPartialRouter>());

            _isInitialized = true;
        }

        private void OnEpiserverRoutesRegistered(object sender, RouteRegistrationEventArgs routeRegistrationEventArgs)
        {
            RouteTable.Routes.MapSiteCategoryRoute("sitecategories", "{language}/{node}/{partial}/{action}", new { action = "index" }, sd => sd.SiteAssetsRoot);
            RouteTable.Routes.MapGlobalCategoryRoute("sharedcategories", "{language}/{node}/{partial}/{action}", new {action = "index"}, sd => sd.GlobalAssetsRoot);
        }

        public void Uninitialize(InitializationEngine context)
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.CreatingContent -= OnCreatingContent;
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            ConfigureContainer(context.Services);
        }

        private static void ConfigureContainer(IServiceConfigurationProvider services)
        {
            services.AddSingleton<ICategoryContentLoader, DefaultCategoryContentLoader>();
            services.AddSingleton<IContentInCategoryLocator, DefaultContentInCategoryLocator>();
            services.AddScoped<ICategoryRouteHelper, DefaultCategoryRouteHelper>();
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