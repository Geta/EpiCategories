using System;
using System.Collections.Generic;
using EPiServer.Cms.Shell.UI.CompositeViews.Internal;
using EPiServer.Cms.Shell.UI.UIDescriptors;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Web;

namespace Geta.EpiCategories
{
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class CategoryContentRepositoryDescriptor : ContentRepositoryDescriptorBase
    {
        public static string RepositoryKey = "categories";

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

        public override IEnumerable<ContentReference> Roots
        {
            get
            {
                var list = new List<ContentReference>
                {
                    SiteDefinition.Current.GlobalAssetsRoot
                };

                if (SiteDefinition.Current.GlobalAssetsRoot != SiteDefinition.Current.SiteAssetsRoot)
                    list.Add(SiteDefinition.Current.SiteAssetsRoot);

                return list;
            }
        }

        public override IEnumerable<Type> MainNavigationTypes => new[]
        {
            typeof(CategoryData)
        };

        public override IEnumerable<string> MainViews => new []
        {
            HomeView.ViewName
        };

        public override string SearchArea => "CMS/categories";

        public override string CustomNavigationWidget => "geta-epicategories/component/CategoryNavigationTree";

        public override string CustomSelectTitle => LocalizationService.Current.GetString("/contentrepositories/categories/customselecttitle");
    }
}