define("geta-epicategories/widget/CategorySelectorDialog", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",

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

    _LayoutWidget,

    Button,

    TypeDescriptorManager,
    epi,
    dependency,
    CategorySelectionTree,
    ContextualContentForestStoreModel,
    SearchBox
) {

    return declare([_LayoutWidget], {
        allowedTypes: null,
        model: null,
        restrictedTypes: null,
        showSearchBox: true,
        searchArea: 'cms/categories',
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
                            this._appendValue(item.metadata.id);
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
                roots: roots,
                allowedTypes: this.allowedTypes,
                restrictedTypes: this.restrictedTypes,
                typeIdentifiers: typeIdentifiers
            });

            this.addChild(this.tree);
        },

        _appendValue: function (value) {
            var contentLinks = this.get('value');

            if (contentLinks.indexOf(value) !== -1) {
                return;
            }

            contentLinks.push(value);
            this.set('value', contentLinks);
            this.tree.selectNodeById(value);
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
