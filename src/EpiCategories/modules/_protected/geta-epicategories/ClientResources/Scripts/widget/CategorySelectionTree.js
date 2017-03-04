define("geta-epicategories/widget/CategorySelectionTree", [
// dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/connect",
    "dojo/dom-class",
    "dojo/when",
    "dojo/promise/all",
    "dojo/Deferred",
    "dojo/Evented",
// epi
    "epi/shell/ClipboardManager",
    "epi/shell/selection",
    "epi/shell/command/_WidgetCommandProviderMixin",
    "epi/shell/widget/ContextMenu",
    "epi-cms/widget/ContentTree",
    "geta-epicategories/widget/CategoryForestStoreModel",
    "epi/shell/TypeDescriptorManager",
    "epi/epi",
// geta
    "geta-epicategories/widget/CategorySelectionTreeNode",
    "geta-epicategories/widget/CategoryContentContextMenuCommandProvider"
],

function (
// dojo
    array,
    declare,
    lang,
    connect,
    domClass,
    when,
    promiseAll,
    Deferred,
    Evented,
// epi
    ClipboardManager,
    Selection,
    _WidgetCommandProviderMixin,
    ContextMenu,
    ContentTree,
    CategoryForestStoreModel,
    TypeDescriptorManager,
    epi,
// geta
    CategorySelectionTreeNode,
    ContentContextMenuCommandProvider
) {

    return declare([ContentTree, _WidgetCommandProviderMixin, Evented], {
        _clipboardManager: null,
        _contextMenu: null,
        _contextMenuCommandProvider: null,
        _focusNode: null,

        categorySettings: null,
        contextMenuCommandProvider: ContentContextMenuCommandProvider,
        nodeConstructor: CategorySelectionTreeNode,
        selectedContentLinks: null,
        selection: null,
        showRoot: false,

        constructor: function () {
            this.selectedContentLinks = [];
        },

        postMixInProperties: function () {
            this.inherited(arguments);

            this._clipboardManager = new ClipboardManager();
            this.selection = new Selection();

            //Create the context menu command provider
            this._contextMenuCommandProvider = new this.contextMenuCommandProvider({
                allowedTypes: this.allowedTypes,
                treeModel: this.model,
                clipboardManager: this._clipboardManager,
                repositoryKey: this.repositoryKey,
                restrictedTypes: this.restrictedTypes
            });

            this._contextMenuCommandProvider.on('onCreateCategoryCommandExecuted', lang.hitch(this, function(command) {
                this.emit('onCreateCategoryCommandExecuted', command);
            }));

            this._contextMenuCommandProvider.on('onNewCategoryCreated', lang.hitch(this, function(category) {
                this.emit('onNewCategoryCreated', category);
            }));

            this.model.roots = this.roots;
            this.model.notAllowToDelete = this.settings.preventDeletionFor;
            this.model.notAllowToCopy = this.settings.preventCopyingFor;
        },

        expandSelectedNodes: function () {
            when(this.onLoadDeferred, lang.hitch(this, function () {
                array.forEach(this.selectedContentLinks, function (contentLink) {
                    this.selectNodeById(contentLink, false);
                }, this);
            }));
        },

        getCommandModel: function (selectedTreeNode) {
            return selectedTreeNode && selectedTreeNode.item;
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

            node.on("onContextMenuClick", lang.hitch(this, function (node) {
                this._updateGlobalToolbarButtons(node.node);
                this._showContextMenu(node);
                this.set("_focusNode", node.node);
            }));

            return node;
        },

        _expandExtraNodes: function () {
            this.expandSelectedNodes();
            return this.inherited(arguments);
        },

        _getSelectionData: function (/*dojo/data/Item*/itemData) {
            // summary:
            //      Return selection data
            // tags:
            //      private

            return itemData ? [{ type: "epi.cms.contentdata", data: itemData }] : [];
        },

        _isItemSelectable: function (item) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([item.typeIdentifier], this.typeIdentifiers, this.restrictedTypes);

            return acceptedTypes.length > 0;
        },

        _onContextMenuClose: function () {
            // summary:
            //      Handles context menu close event
            // tags:
            //      private

            this._removeHighlightClass();
        },

        _onContextMenuOpen: function () {
            if (this._focusNode) {
                domClass.add(this._focusNode.rowNode, "dijitTreeRowSelected");
            }
        },

        _onNodeMouseEnter: function (node, evt) {
            this.inherited(arguments);
            node.showContextMenu(true);
        },

        _onNodeMouseLeave: function (node, evt) {
            this.inherited(arguments);
            node.showContextMenu(false);
        },

        _onNodeSelectChanged: function (checked, item) {
            if (!this.getNodeById(item.contentLink)) {
                return;
            }

            if (this.selectedContentLinks == null) {
                this.selectedContentLinks = [];
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

        _removeHighlightClass: function () {
            if (this._focusNode && this._focusNode !== this.selectedNode && this._focusNode.rowNode) {
                domClass.remove(this._focusNode.rowNode, "dijitTreeRowSelected");
            }
        },

        _setSelectedContentLinksAttr: function (value) {
            if (!lang.isArray(value)) {
                value = [value];
            }

            var filteredValue = array.filter(value, function (v) {
                return !!v;
            });

            this._set('selectedContentLinks', filteredValue);
        },

        _showContextMenu: function (evt) {
            //Create the context menu if used for the first time
            if (!this._contextMenu) {
                this.own(
                    this._contextMenu = new ContextMenu({ leftClickToOpen: true, category: "context", popupParent: this }),
                    connect.connect(this._contextMenu, "onClose", this, "_onContextMenuClose"),
                    connect.connect(this._contextMenu, "onOpen", this, "_onContextMenuOpen")
                );
                this._contextMenu.addProvider(this._contextMenuCommandProvider);
                this._contextMenu.startup();
            }

            //Open the context menu
            this._contextMenu.scheduleOpen(evt.target, null, { x: evt.x, y: evt.y });
        },

        _updateGlobalToolbarButtons: function (targetNode) {
            var node = targetNode || this.get("selectedNode"),
                selected = node ? this._getSelectionData(node.item) : [],
                commandModel = this.getCommandModel(node);

            //Bind the item
            this._contextMenuCommandProvider.updateCommandModel(commandModel);
            this.selection.set("data", selected);
        }
    });
});
