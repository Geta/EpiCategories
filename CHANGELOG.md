## Changelog

### Changes in version 1.2.11
1. Added new setting to allow hiding of disallowed root categories, see [#31 Category selection popup shows all categories instead of showing only allowed category](https://github.com/Geta/EpiCategories/issues/31).)

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
