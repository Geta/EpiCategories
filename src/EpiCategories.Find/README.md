# EpiCategories.Find

## Description
CMS category search provider and extensions methods for projects with EpiCategories and Episerver Find.

## Features
* CMS search provider powered by Episerver Find.
* Category filter and facet extension methods for ITypeSearch and IHasFacetResults.

## How to install
Install NuGet package from Episerver NuGet Feed:

	Install-Package Geta.EpiCategories.Find

## How to use
### ITypeSearch<T> extension methods:

	ITypeSearch<T> FilterByCategories<T>(this ITypeSearch<T> search, IEnumerable<ContentReference> categories) where T : ICategorizableContent
	ITypeSearch<T> FilterHitsByCategories<T>(this ITypeSearch<T> search, IEnumerable<ContentReference> categories) where T : ICategorizableContent
	ITypeSearch<T> ContentCategoriesFacet<T>(this ITypeSearch<T> request) where T : ICategorizableContent

### IHasFacetResults<T> extension methods:

	IEnumerable<ContentCount> ContentCategoriesFacet<T>(this IHasFacetResults<T> result) where T : ICategorizableContent

## Package maintainer
https://github.com/MattisOlsson