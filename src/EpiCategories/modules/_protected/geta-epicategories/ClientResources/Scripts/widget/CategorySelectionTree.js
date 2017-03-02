define("geta-epicategories/widget/CategorySelectionTree", [
// dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/connect",
    "dojo/when",
    "dojo/promise/all",
    "dojo/Deferred",

    "epi-cms/widget/ContentTree",
    "epi-cms/widget/ContentForestStoreModel",
    "epi/shell/TypeDescriptorManager",
    "epi/epi",

    "geta-epicategories/widget/CategorySelectionTreeNode",
// resources
    "epi/i18n!epi/cms/nls/episerver.cms.components.createcategory",
    "epi/i18n!epi/cms/nls/episerver.cms.components.categorytree",
    "epi/i18n!epi/cms/nls/episerver.shared.header"
],

function (
// dojo
    array,
    declare,
    lang,
    connect,
    when,
    promiseAll,
    Deferred,

    ContentTree,
    ContentForestStoreModel,
    TypeDescriptorManager,
    epi,

    CategorySelectionTreeNode,
// resources
    resCreateCategory,
    res
) {

    return declare([ContentTree], {
        res: res,
        showRoot: false,
        nodeConstructor: CategorySelectionTreeNode,
        selectedContentLinks: null,

        constructor: function () {
            this.selectedContentLinks = [];
        },

        getNodeById: function (contentLink) {
            var nodes = this.getNodesByItem(contentLink);

            if (!nodes) {
                return null;
            }

            return nodes[0];
        },

        _onNodeSelectChanged: function (checked, item) {
            if (!this.getNodeById(item.contentLink)) {
                return;
            }

            var index = this.selectedContentLinks.indexOf(item.contentLink),
                exist = index !== -1;

            if (checked && !exist) {
                this.selectedContentLinks.push(item.contentLink);
            }

            if (!checked && exist) {
                this.selectedContentLinks.splice(index, 1);
            }
        },

        _expandExtraNodes: function() {
            this.expandSelectedNodes();
            return this.inherited(arguments);
        },

        selectNodeById: function (contentLink) {
            this.selectContent(contentLink, false).then(function (node) {
                node.set('checked', true);
                node.setSelected(false);
            });
        },

        expandSelectedNodes: function () {
            when(this.onLoadDeferred, lang.hitch(this, function() {
                array.forEach(this.selectedContentLinks, function (contentLink) {
                    this.selectNodeById(contentLink);
                }, this);
            }));
        },

        _createTreeNode: function () {
            var node = this.inherited(arguments);

            if (this._isItemSelectable(node.item)) {
                node.connect(node, "onNodeSelectChanged", lang.hitch(this, function (checked, item) {
                    this._onNodeSelectChanged(checked, item);
                }));
            }

            return node;
        },

        _createTreeModel: function () {
            return new ContentForestStoreModel({
                roots: this.roots,
                typeIdentifiers: this.typeIdentifiers
            });
        },

        _setSelectedContentLinksAttr: function (value) {
            this._set('selectedContentLinks', value);
        },
        
        _isItemSelectable: function (item) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([item.typeIdentifier], this.typeIdentifiers, ['episerver.core.contentfolder']);

            return acceptedTypes.length > 0;
        }
    });
});
