using System;
using System.Collections.Generic;
using EPiServer.Cms.Shell.UI.UIDescriptors;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace Geta.EpiCategories
{
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class CategoryContentRepositoryDescriptor : PageRepositoryDescriptor
    {
        public new static string RepositoryKey = "categories";

        public override string Key => RepositoryKey;

        public override string Name => LocalizationService.Current.GetString("/contentrepositories/categories/name");

        public override IEnumerable<Type> CreatableTypes => new[]
        {
            typeof (CategoryData)
        };

        public override IEnumerable<Type> ContainedTypes => new[]
        {
            typeof (CategoryData)
        };

        public override IEnumerable<Type> LinkableTypes => new[]
        {
            typeof (CategoryData)
        };

        public override IEnumerable<ContentReference> Roots => new[]
        {
            ServiceLocator.Current.GetInstance<ICategoryContentRepository>().GetRootLink()
        };

        public override IEnumerable<Type> MainNavigationTypes => new[]
        {
            typeof(CategoryData)
        };

        public override string SearchArea => "cms/categories";

        public override string CustomNavigationWidget => "geta-epicategories/component/CategoryNavigationTree";

        public override string CustomSelectTitle => LocalizationService.Current.GetString("/contentrepositories/categories/customselecttitle");
    }
}