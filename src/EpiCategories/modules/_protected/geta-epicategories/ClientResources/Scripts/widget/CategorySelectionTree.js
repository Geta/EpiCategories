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

        _loadedAncestors: null,

        constructor: function () {
            this.selectedContentLinks = [];
            this._loadedAncestors = [];
        },

        postCreate: function () {
            this.inherited(arguments);
            this.connect(this, "onOpen", lang.hitch(this, this._onNodeOpen));
        },

        startup: function() {
            if (this._started) {
                return;
            }

            this.inherited(arguments);

            // Disabling multiselect in tree
            this.dndController.singular = true;
        },

        getNodeById: function (contentLink) {
            var nodes = this.getNodesByItem(contentLink);

            if (!nodes) {
                return null;
            }

            return nodes[0];
        },

        _onNodeOpen: function (item, node) {
            this.set("toggleSelectNodes", node, this.selectedContentLinks);
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

        expandSelectedNodes: function () {
            var dfdList = [];

            array.forEach(this.selectedContentLinks, function (contentLink) {
                var dfd = new Deferred();
                dfdList.push(dfd);

                this.model.getAncestors(contentLink, lang.hitch(this, function (ancestors) {
                    ancestors.splice(0, 2);
                    this._onAncestorsLoaded(ancestors, dfd);
                }));
            }, this);

            promiseAll(dfdList).then(lang.hitch(this, function () {
                this._expandAncestors(lang.clone(this._loadedAncestors));
            }));
        },

        _onAncestorsLoaded: function(ancestors, dfd) {
            array.forEach(ancestors, function(ancestor) {
                if (this._loadedAncestors.indexOf(ancestor.contentLink) === -1) {
                    this._loadedAncestors.push(ancestor.contentLink);
                }
            }, this);

            dfd.resolve();
        },

        _setToggleSelectNodesAttr: function (parentNode, toggleNodeIds) {
            if (!parentNode || !lang.isArray(toggleNodeIds)) {
                return;
            }

            var checked = false;

            array.forEach(parentNode.getChildren(), function (childNode) {
                if (childNode) {
                    checked = toggleNodeIds.indexOf(childNode.item.contentLink) !== -1;
                    childNode.set("checked", checked);

                    if (childNode.hasChildren()) {
                        this.set("toggleSelectNodes", childNode, toggleNodeIds);
                    }
                }
            }, this);
        },

        _expandAncestors: function (ancestors) {
            if (ancestors.length === 0) {
                return;
            }

            var ancestor = ancestors[0];
            var node = this.getNodeById(ancestor);
            ancestors.splice(0, 1);

            if (node && !node.isExpanded) {
                this._expandNode(node).then(lang.hitch(this, function() {
                    this._expandAncestors(ancestors);
                }));
            } else {
                this._expandAncestors(ancestors);
            }
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
            this.set('toggleSelectNodes', this.rootNode, this.selectedContentLinks);
        },
        
        _isItemSelectable: function (item) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([item.typeIdentifier], this.typeIdentifiers, ['episerver.core.contentfolder']);

            return acceptedTypes.length > 0;
        }
    });
});
