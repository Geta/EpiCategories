using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace Geta.EpiCategories
{
    [Component]
    public class CategoryTreeComponent : ComponentDefinitionBase
    {
        private string _title;

        public CategoryTreeComponent() : this(ServiceLocator.Current.GetInstance<CategorySettings>())
        {
        }

        public CategoryTreeComponent(CategorySettings categorySettings) : base("epi-cms/component/MainNavigationComponent")
        {
            LanguagePath = "/episerver/cms/components/pagetree";

            PlugInAreas = new[]
            {
                PlugInArea.AssetsDefaultGroup,
                PlugInArea.NavigationDefaultGroup
            };

            Categories = new[] { "content" };
            SortOrder = 105;
            Settings.Add(new Setting("repositoryKey", CategoryContentRepositoryDescriptor.RepositoryKey));
            Settings.Add(new Setting("categorySettings", categorySettings));
        }

        public override string Title
        {
            get
            {
                string title = LocalizationService.GetString("/admin/categories/heading");

                if (string.IsNullOrEmpty(title) == false)
                    return title;

                return _title;
            }
            set
            {
                _title = value;
            }
        }
    }
}