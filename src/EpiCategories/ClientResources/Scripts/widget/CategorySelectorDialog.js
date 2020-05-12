define("geta-epicategories/widget/CategorySelectorDialog", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/Evented",

    "dijit/layout/_LayoutWidget",
    "dijit/form/Button",

    "epi/shell/TypeDescriptorManager",
    "epi",
    "epi/dependency",
    "geta-epicategories/widget/CategorySelectionTree",
    "epi-cms/widget/ContextualContentForestStoreModel",
    "epi-cms/widget/SearchBox"
],

function (
    array,
    declare,
    lang,
    Evented,

    _LayoutWidget,

    Button,

    TypeDescriptorManager,
    epi,
    dependency,
    CategorySelectionTree,
    ContextualContentForestStoreModel,
    SearchBox
) {

    return declare([_LayoutWidget, Evented], {
        allowedTypes: null,
        categorySettings: null,
        model: null,
        repositoryKey: null,
        restrictedTypes: null,
        showSearchBox: true,
        searchArea: 'cms/categories',
        settings: null,
        value: [],
        _typesToDisplay: null,

        constructor: function () {
            this.inherited(arguments);
            this.value = [];
        },

        buildRendering: function () {
            this.inherited(arguments);

            var typeIdentifiers = this._getTypesToDisplay(),
                model = this.model,
                roots = this.roots || (model && model.roots);

            if (!model) {
                model = new ContextualContentForestStoreModel({
                    roots: roots,
                    typeIdentifiers: typeIdentifiers,
                    showAllLanguages: this.showAllLanguages
                });
            }

            if (this.showSearchBox && this.searchArea) {
                this._searchBox = new SearchBox({
                    innerSearchBoxClass: "epi-search--full-width",
                    triggerContextChange: false,
                    parameters: {
                        allowedTypes: this.allowedTypes,
                        restrictedTypes: this.restrictedTypes
                    },
                    onItemAction: lang.hitch(this, function (item) {
                        if (item && item.metadata && this._checkAcceptance(item.metadata.typeIdentifier)) {
                            this.appendValue(item.metadata.id);
                        }
                    })
                });
                this._searchBox.set("area", this.searchArea);
                this._searchBox.set("searchRoots", this.roots);

                var searchBox = this._searchBox;
                model.getRoots(true).then(function (roots) {
                    searchBox.set("searchRoots", roots.join(","));
                });

                this.addChild(this._searchBox);
            }

            this.tree = new CategorySelectionTree({
                categorySettings: this.categorySettings,
                roots: roots,
                repositoryKey: this.repositoryKey,
                allowedTypes: lang.clone(this.allowedTypes),
                restrictedTypes: lang.clone(this.restrictedTypes),
                typeIdentifiers: typeIdentifiers,
                settings: this.settings
            });

            this.tree.on('onCreateCategoryCommandExecuted', lang.hitch(this, function(command) {
                this.emit('onCreateCategoryCommandExecuted', command);
            }));

            this.tree.on('onNewCategoryCreated', lang.hitch(this, function(category) {
                this.emit('onNewCategoryCreated', category);
            }));

            this.addChild(this.tree);
        },

        appendValue: function (value) {
            var contentLinks = this.get('value') || [];

            if (contentLinks.indexOf(value) === -1) {
                contentLinks.push(value);
                this.set('value', contentLinks);
            }
        },

        onShow: function () {
        },

        _checkAcceptance: function (typeIdentifier) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([typeIdentifier], this.allowedTypes, this.restrictedTypes);

            return acceptedTypes.length > 0;
        },

        _getValueAttr: function () {
            return this.tree.get('selectedContentLinks');
        },

        _getTypesToDisplay: function () {
            if (this.categorySettings.hideDisallowedRootCategories) {
                return this.allowedTypes;
            }

            if (!this._typesToDisplay) {

                var typesToDisplay = [];
                this._getContainerTypesRecursive(this.allowedTypes, typesToDisplay);
                this._typesToDisplay = typesToDisplay;
            }

            return this._typesToDisplay;
        },

        _getContainerTypesRecursive: function (types, results, checkedTypes) {
            if (!checkedTypes) {
                checkedTypes = [];
            }

            array.forEach(types, lang.hitch(this, function (type) {
                // To avoid infinite recursion, check if this type is already processed
                if (array.indexOf(checkedTypes, type) === -1) {
                    // Mark as checked
                    checkedTypes.push(type);

                    // Add the type itself if it isn't added already
                    if (array.indexOf(results, type) === -1) {
                        results.push(type);
                    }

                    // If there are any container types, process them as well
                    var containerTypesForType = TypeDescriptorManager.getValue(type, "containerTypes");

                    if (containerTypesForType) {
                        this._getContainerTypesRecursive(containerTypesForType, results, checkedTypes);
                    }
                }
            }));
        },

        _setValueAttr: function (value) {
            this.tree.set('selectedContentLinks', value);
        }
    });
});
