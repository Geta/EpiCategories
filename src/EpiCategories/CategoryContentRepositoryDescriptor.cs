using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Cms.Shell.UI.CompositeViews.Internal;
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
        public CategoryContentRepositoryDescriptor(CategorySettings categorySettings)
        {
            CategorySettings = categorySettings;
        }

        public readonly CategorySettings CategorySettings;

        public static string RepositoryKey = "categories";

        public override string Key => RepositoryKey;

        public override string Name => LocalizationService.Current.GetString("/admin/categories/heading");

        public virtual string CreatingTypeIdentifier => typeof (CategoryData).FullName.ToLowerInvariant();

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
                    new ContentReference(this.CategorySettings.GlobalCategoriesRoot)
                };

                if (SiteDefinition.Current.GlobalAssetsRoot != SiteDefinition.Current.SiteAssetsRoot)
                {
                    list.Add(new ContentReference(this.CategorySettings.SiteCategoriesRoot));
                }

                return list;
            }
        }

        public override IEnumerable<Type> MainNavigationTypes => new[]
        {
            typeof(CategoryData)
        };

        public override IEnumerable<string> PreventContextualContentFor => Roots.Select(x => x.ToReferenceWithoutVersion().ToString());
        public override IEnumerable<string> PreventCopyingFor => PreventContextualContentFor;
        public override IEnumerable<string> PreventDeletionFor => PreventContextualContentFor;

        public override IEnumerable<string> MainViews => new []
        {
            HomeView.ViewName
        };

        public override string SearchArea => "CMS/categories";

        public override string CustomNavigationWidget => "geta-epicategories/component/CategoryNavigationTree";
    }
}