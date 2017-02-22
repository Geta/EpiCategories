using EPiServer.Cms.Shell.UI.Components;
using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace Geta.EpiCategories
{
    [Component]
    public class CategoryTreeComponent : ComponentDefinitionBase
    {
        public CategoryTreeComponent() : base("epi-cms/component/MainNavigationComponent")
        {
            LanguagePath = "/episerver/cms/components/categorytree";
            PlugInAreas = new[]
            {
                PlugInArea.AssetsDefaultGroup,
                PlugInArea.NavigationDefaultGroup
            };
            Categories = new[] { "content" };
            SortOrder = 115;
            Settings.Add(new Setting("repositoryKey", CategoryContentRepositoryDescriptor.RepositoryKey));
        }
    }
}