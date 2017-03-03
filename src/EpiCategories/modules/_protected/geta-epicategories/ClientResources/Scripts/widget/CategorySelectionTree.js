define("geta-epicategories/widget/CategorySelectionTree", [
// dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/connect",
    "dojo/when",
    "dojo/promise/all",
    "dojo/Deferred",
// epi
    "epi-cms/widget/ContentTree",
    "geta-epicategories/widget/CategoryForestStoreModel",
    "epi/shell/TypeDescriptorManager",
    "epi/epi",
// geta
    "geta-epicategories/widget/CategorySelectionTreeNode"
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
// epi
    ContentTree,
    CategoryForestStoreModel,
    TypeDescriptorManager,
    epi,
// geta
    CategorySelectionTreeNode
) {

    return declare([ContentTree], {
        categorySettings: null,
        nodeConstructor: CategorySelectionTreeNode,
        selectedContentLinks: null,
        selection: null,
        showRoot: false,

        constructor: function () {
            this.selectedContentLinks = [];
        },

        expandSelectedNodes: function () {
            when(this.onLoadDeferred, lang.hitch(this, function () {
                array.forEach(this.selectedContentLinks, function (contentLink) {
                    this.selectNodeById(contentLink, false);
                }, this);
            }));
        },

        getNodeById: function (contentLink) {
            var nodes = this.getNodesByItem(contentLink);

            if (!nodes) {
                return null;
            }

            return nodes[0];
        },

        selectNodeById: function (contentLink, highlight) {
            this.selectContent(contentLink, true).then(function (node) {
                node.setSelected(highlight);
                node.set('checked', true);
            });
        },

        _createTreeModel: function () {
            return new CategoryForestStoreModel({
                categorySettings: this.categorySettings,
                roots: this.roots,
                typeIdentifiers: this.typeIdentifiers
            });
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

        _expandExtraNodes: function () {
            this.expandSelectedNodes();
            return this.inherited(arguments);
        },

        _isItemSelectable: function (item) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([item.typeIdentifier], this.typeIdentifiers, this.restrictedTypes);

            return acceptedTypes.length > 0;
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

        _setSelectedContentLinksAttr: function (value) {
            this._set('selectedContentLinks', value);
        }
    });
});
