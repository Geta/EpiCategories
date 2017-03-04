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

        constructor: function () {
            this.inherited(arguments);
            this.value = [];
        },

        buildRendering: function () {
            this.inherited(arguments);

            var typeIdentifiers = this.allowedTypes,
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
                allowedTypes: this.allowedTypes,
                restrictedTypes: this.restrictedTypes,
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

        onShow: function () {
            this.tree.expandSelectedNodes();
        },

        appendValue: function (value) {
            var contentLinks = this.get('value') || [];

            this.tree.selectNodeById(value, true);

            if (contentLinks.indexOf(value) !== -1) {
                return;
            }

            contentLinks.push(value);
            this.set('value', contentLinks);
        },

        _setValueAttr: function (value) {
            this.tree.set('selectedContentLinks', value);
        },

        _checkAcceptance: function (typeIdentifier) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([typeIdentifier], this.allowedTypes, this.restrictedTypes);

            return acceptedTypes.length > 0;
        },

        _getValueAttr: function () {
            return this.tree.get('selectedContentLinks');
        }
    });
});
