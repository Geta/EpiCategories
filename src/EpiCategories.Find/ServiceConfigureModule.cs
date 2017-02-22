using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StructureMap;

namespace Geta.EpiCategories.Find
{
    [InitializableModule]
    [ModuleDependency(typeof(CategoryInitializationModule))]
    public class ServiceConfigureModule : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(ConfigureContainer);
        }

        private void ConfigureContainer(ConfigurationExpression container)
        {
            container.For<IContentInCategoryLocator>().Singleton().Use<FindContentInCategoryLocator>();
        }
    }
}