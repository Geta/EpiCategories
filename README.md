# EpiCategories

## Description
An alternative to Episerver's default category functionality, where categories are instead stored as localizable IContent.

## Features
* Localization (no more language XML files)
* More user friendly edit UI
* Access rights support (some editors should perhaps have limited category access)
* Shared and site specific categories in multisite solutions
* Partial routing of category URL segments
  
## How to install
Install NuGet package from Episerver NuGet Feed:

	Install-Package Geta.EpiCategories
	
## CMS search providers (Episerver Find or Episerver Search)
If you want to install a CMS search provider you can choose from Geta.EpiCategories.Find or Geta.EpiCategories.Search package depending on if you have Episerver Find installed in your project or not:

	Install-Package Geta.EpiCategories.Search
	
or:

	Install-Package Geta.EpiCategories.Find

  
## How to use
Start by creating a category content type that inherits from CategoryData. You can have multiple.

	[ContentType]
	public class BasicCategory : CategoryData
	{
	}
	
	[ContentType]
	public class ExtendedCategory : BasicCategory
	{
		[CultureSpecific]
		public virtual XhtmlString MainBody { get; set; }
	}

CategoryData looks like this:

    public class CategoryData : StandardContentBase, IRoutable
    {
        [UIHint(UIHint.PreviewableText)]
        [CultureSpecific]
        public virtual string RouteSegment { get; set; }

        [Display(Order = 20)]
        [UIHint(UIHint.LongString)]
        [CultureSpecific]
        public virtual string Description { get; set; }

        [Display(Order = 30)]
        [CultureSpecific]
        public virtual bool IsSelectable { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            IsSelectable = true;
        }
    }
	
### Edit categories
Instead of going to admin mode to manage categories, you now do it in edit mode, under the "Categories" tab in the main navigation component to the left. You work with them like normal pages, and it's possible to translate them. You can create categories that are shared between multiple sites or you can create site specific categories.

![ScreenShot](/docs/extended-category-tree.jpg)

### ICategorizableContent interface
Implement ICategorizableContent on your content type class to categorize your content.

	public interface ICategorizableContent
	{
		IList<ContentReference> Categories { get; set; }
	}
	
	public class MyPageType : PageData, ICategorizableContent
	{
		[Categories]
		public virtual IList<ContentReference> Categories { get; set; }
	}
	
Above property will look familiar if you have used standard Episerver categories before.

![ScreenShot](/docs/category-selector.jpg)
![ScreenShot](/docs/category-selector-dialog.jpg)

There is a context menu in the selector where you quickly can create and auto publish a new category and automatically get back to the selector with the new category selected:

![ScreenShot](/docs/category-selector-dialog-create-new.jpg)

If you prefer to use the native content reference list editor for your categories you can skip the CategoriesAttribute:

	[AllowedTypes(typeof(CategoryData))]
	public virtual IList<ContentReference> Categories { get; set; }

![ScreenShot](/docs/content-reference-list.jpg)

If you want a single category on your content type just add a ContentReference property:

	[UIHint(CategoryUIHint.Category)]
	public virtual ContentReference MainCategory { get; set; }

### IEnumerable&lt;ContentReference> extension methods
The following extension methods are included:

1. MemberOf(this IEnumerable&lt;ContentReference> contentLinks, ContentReference contentReference)
2. MemberOfAny(this IEnumerable&lt;ContentReference> contentLinks, IEnumerable&lt;ContentReference> otherContentLinks)
3. MemberOfAll(this IEnumerable&lt;ContentReference> contentLinks, IEnumerable&lt;ContentReference> otherContentLinks)

### ICategoryContentLoader interface
There is an implementation of ICategoryContentLoader (note that in 1.0.0 it is mistakenly named ICategoryContentRepository) that you can use to load categories:

	public interface ICategoryContentLoader
	{
			T Get<T>(ContentReference categoryLink) where T : CategoryData;

			IEnumerable<T> GetChildren<T>(ContentReference parentCategoryLink) where T : CategoryData;

			IEnumerable<T> GetChildren<T>(ContentReference parentCategoryLink, CultureInfo culture) where T : CategoryData;

			IEnumerable<T> GetChildren<T>(ContentReference parentCategoryLink, LoaderOptions loaderOptions) where T : CategoryData;

			T GetFirstBySegment<T>(string urlSegment) where T : CategoryData;

			T GetFirstBySegment<T>(string urlSegment, CultureInfo culture) where T : CategoryData;

			T GetFirstBySegment<T>(string urlSegment, LoaderOptions loaderOptions) where T : CategoryData;

			T GetFirstBySegment<T>(ContentReference parentLink, string urlSegment, LoaderOptions loaderOptions) where T : CategoryData;

			IEnumerable<T> GetGlobalCategories<T>() where T : CategoryData;

			IEnumerable<T> GetGlobalCategories<T>(CultureInfo culture) where T : CategoryData;

			IEnumerable<T> GetGlobalCategories<T>(LoaderOptions loaderOptions) where T : CategoryData;

			IEnumerable<T> GetSiteCategories<T>() where T : CategoryData;

			IEnumerable<T> GetSiteCategories<T>(CultureInfo culture) where T : CategoryData;

			IEnumerable<T> GetSiteCategories<T>(LoaderOptions loaderOptions) where T : CategoryData;

			bool TryGet<T>(ContentReference categoryLink, out T category) where T : CategoryData;
	}

Inject it in your controller as you are used to:
	
	public class MyController : Controller
	{
		private readonly ICategoryContentLoader _categoryLoader;
		
		public MyController(ICategoryContentLoader categoryLoader)
		{
			_categoryLoader = categoryLoader;	
		}
	}
	
### IContentInCategoryLocator interface
You can use IContentInCategoryLocator to find content in certain categories:

    public interface IContentInCategoryLocator
    {
        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent;
		
        IEnumerable<T> GetReferencesToCategories<T>(IEnumerable<ContentReference> categories) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetReferencesToCategories<T>(IEnumerable<ContentReference> categories, CultureInfo culture) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetReferencesToCategories<T>(IEnumerable<ContentReference> categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent;
    }

## Routing
Two routes are mapped during initialization. One for site categories and one for global categories. This means you can create templates for your category content types. They are routed in a similar way as normal pages. You can set the root segment on the "For This Site" and "For All Sites" category nodes in the Categories tree.

![ScreenShot](/docs/for-this-site.jpg)

Using above example, the URL "/topics/sports/" would be routed to the site category called "Sports".

### ICategoryRoutableContent interface
Implement this on your content type and it will be possible to route category URL segments with the help of a partial router shipped in this package. Let's say you have an article list page with the URL "/articles/" on your site. If you have a category with the url segment of "sports", you can add it to the end of your list page URL, "/articles/sports/", and the category data will be added to the route values with the key "currentCategory". Your controller action method could look something like this:

	public ActionResult Index(ArticleListPage currentPage, CategoryData currentCategory)
	{
	}

You can also have multiple category URL segments separated with the configured category separator: /articles/entertainment__sports/.

	public ActionResult Index(ArticleListPage currentPage, IList<CategoryData> currentCategories) // currentCategories will now contain "Sports" and "Entertainment" categories.
	{
	}

Default category separator is "__" and you can change it by adding an appSetting in web.config:

	<add key="GetaEpiCategories:CategorySeparator" value="__" />
	
There is a couple of UrlHelper and UrlResolver extension methods included to get content URL with category segment added:

	@Url.CategoryRoutedContentUrl(/*ContentReference*/ contentLink, /*ContentReference*/ categoryContentLink) // Single category
	@Url.CategoryRoutedContentUrl(/*ContentReference*/ contentLink, /*IEnumerable<ContentReference>*/ categoryContentLinks) // Multiple categories

	@UrlResolver.Current.GetCategoryRoutedUrl(/*ContentReference*/ contentLink, /*ContentReference*/ categoryContentLink) // Single category
	@UrlResolver.Current.GetCategoryRoutedUrl(/*ContentReference*/ contentLink, /*IEnumerable<ContentReference>*/ categoryContentLinks) // Multiple categories
	
## Show default Episerver category
This package hides the default Episerver category property. You can opt-out from this by adding an app setting:

    <add key="GetaEpiCategories:ShowDefaultCategoryProperty" value="true" />

## Package maintainer
https://github.com/MattisOlsson

## Changelog
### Changes in version 1.2.10
1. Visitor group criteria added, see [pull request #26: added criterion](https://github.com/Geta/EpiCategories/pull/26). (thanks to Mark Hall aka [lunchin](https://github.com/lunchin))

### Changes in version 1.2.9
1. Implemented issue [#20 Setting to hide category in LinkModel dialog](https://github.com/Geta/EpiCategories/issues/20)

### Changes in version 1.2.8
1. Added app setting to show default Episerver category property:

    	<add key="GetaEpiCategories:ShowDefaultCategoryProperty" value="true" />

### Changes in version 1.2.0
1. Updated to Episerver 11 (thanks [nolmsted](https://github.com/nolmsted))
2. Moved CategoriesSearchProvider into it's own project to follow Episerver's decoupling of search. (thanks [nolmsted](https://github.com/nolmsted))

### Changes in version 1.1.1

1. Fixed issue [#1 Add-On breaks Media path](https://github.com/Geta/EpiCategories/issues/1).

### New features in version 1.1.0

1. Added support for multiple categories in partial router. See [Routing](#routing) section.

### New features in version 1.0.5

1. Added ability to quickly create and auto publish new categories from selector dialog.
2. Added new method in IContentInCategoryLocator: GetReferencesToCategories. This method finds all content with references to supplied categories.
