# EpiCategories
An alternative to Episerver's default category functionality, where categories are instead stored as localizable IContent.

## Why should you use it?
It has the following advantages over default functionality:

1. Localization (no more language XML files)
2. Access rights support (some editors should perhaps have limited category access)
3. Shared and site specific categories in multisite solutions
4. Partial routing of category URL segments

## How to install
Install NuGet package from Episerver NuGet Feed:

	Install-Package Geta.EpiCategories
  
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
Implement ICategorizableContent on your content type class to categorize your content. This interface contains the Categories property of type ContentCategoryList, which inherits from List&lt;ContentReference>. ContentCategoryList has MemberOf, MemberOfAny and MemberOfAll methods, exactly like CategoryList has.

	public interface ICategorizableContent
	{
		ContentCategoryList Categories { get; set; }
	}
	
	public class MyPageType : PageData, ICategorizableContent
	{
		public virtual ContentCategoryList Categories { get; set; }
	}
	
Above property will look familiar if you have used standard Episerver categories before.

![ScreenShot](/docs/category-selector.jpg)
![ScreenShot](/docs/category-selector-dialog.jpg)

If you prefer to use the native content reference list editor for your categories you can add a UI hint:

	[UIHint(CategoryUIHint.ContentReferenceList)]
	public virtual ContentCategoryList Categories { get; set; }

![ScreenShot](/docs/content-reference-list.jpg)

If you want a single category on your content type just add a ContentReference property:

	[UIHint(CategoryUIHint.Category)]
	public virtual ContentReference MainCategory { get; set; }

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
        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories, CultureInfo culture) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetDescendents<T>(ContentReference contentLink, ContentCategoryList categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, ContentCategoryList categories) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, ContentCategoryList categories, CultureInfo culture) where T : ICategorizableContent, IContent;

        IEnumerable<T> GetChildren<T>(ContentReference contentLink, ContentCategoryList categories, LoaderOptions loaderOptions) where T : ICategorizableContent, IContent;
    }

### ICategoryRoutableContent interface
Implement this on your content type and it will be possible to route category URL segments with the help of a partial router shipped in this package. Let's say you have an article list page with the URL "/articles/" on your site. If you have a category with the url segment of "sports", you can add it to the end of your list page URL, "/articles/sports/", and the category data will be added to the route values with the key "currentCategory". Your controller action method could look something like this:

	public ActionResult Index(ArticleListPage currentPage, CategoryData currentCategory)
	{
	}
	
There is a UrlHelper and UrlResolver extension method included to get content URL with category segment added:

	@Url.CategoryRoutedContentUrl(/*ContentReference*/ contentLink, /*ContentReference*/ categoryContentLink)
	@UrlResolver.Current.GetCategoryRoutedUrl(/*ContentReference*/ contentLink, /*ContentReference*/ categoryContentLink)