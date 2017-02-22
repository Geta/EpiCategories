define("geta-epicategories/contentediting/CategorySelectorDialog", [
// Dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/dom-geometry",
    "dojo/when",

// Dijit
    "dijit/layout/_LayoutWidget",

// EPi
    "epi",
    "epi/shell/TypeDescriptorManager",

    "epi-cms/ApplicationSettings",
    "epi-cms/widget/ContentTree",
    "epi-cms/widget/ContextualContentForestStoreModel",
    "epi-cms/widget/SearchBox",

// EPi Framework
    "epi/shell/widget/_ActionProviderWidget",
    "epi/dependency",

// Resources
    "epi/i18n!epi/cms/nls/episerver.cms.widget.contentselectordialog",

    // c
    "geta-epicategories/component/CategoryNavigationTree"
],

function (
// Dojo
    array,
    declare,
    lang,
    domGeometry,
    when,

// Dijit
    _LayoutWidget,

// EPi
    epi,
    TypeDescriptorManager,

    ApplicationSettings,
    ContentTree,
    ContextualContentForestStoreModel,
    SearchBox,


// EPi Framework
    _ActionProviderWidget,
    dependency,

// Resources
    localization,
    // c
    CategoryNavigationTree
) {
    return declare([_LayoutWidget, _ActionProviderWidget], {

        res: localization,
        roots: null,
        typeIdentifiers: null,
        showButtons: false,
        model: null,
        allowedTypes: null,
        restrictedTypes: null,
        canSelectOwnerContent: true,
        multiRootsMode: false,
        _typesToDisplay: null,
        showAllLanguages: false,
        selectedContentType: null,
        baseClass: "epi-content-selector-dialog",

        repositoryKey: null,
        settings: null,
        disableRestrictedTypes: false,

        postMixInProperties: function () {
            this.inherited(arguments);

            this.contentRepositoryDescriptors = this.contentRepositoryDescriptors || dependency.resolve("epi.cms.contentRepositoryDescriptors");
            var settings = this.contentRepositoryDescriptors[this.repositoryKey];

            this.settings = settings;
            this.roots = settings.roots;
            this.allowedTypes = settings.containedTypes;
            this.preventContextualContentFor = settings.preventContextualContentFor;
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
                    notSupportContextualContents: this.preventContextualContentFor,
                    showAllLanguages: this.showAllLanguages
                });
            }

            model.set("view", "contentselector");

            this.tree = new CategoryNavigationTree({
                roots: roots,
                typeIdentifiers: typeIdentifiers,
                allowManipulation: false,
                model: model,
                allowedTypes: lang.clone(this.allowedTypes),
                restrictedTypes: lang.clone(this.restrictedTypes),
                disableRestrictedTypes: false,
                expandExtraNodeItems: true,
                repositoryKey: this.repositoryKey,
                settings: this.settings,
                selectionMode: true
            });

            this.addChild(this.tree);
            this.connect(this.tree, "onClick", "_onTreeNodeClick");
        },

        layout: function () {
            this.inherited(arguments);

            var treeHeight = this._contentBox.h;

            //Subtract the height of the searchBox if it is created
            if (this._searchBox) {
                treeHeight -= domGeometry.getMarginBox(this._searchBox.domNode).h;
            }

            //Get the margin box for the tree and change the height
            var treeMarginBox = domGeometry.getMarginBox(this.tree.domNode);
            treeMarginBox.h = treeHeight;

            domGeometry.setMarginBox(this.tree.domNode, treeMarginBox);
        },

        _getSettingFromContentType: function () {
            // summary:
            //      Get preventContextualContentFor settings form selected content type
            // tags:
            //      private

            if (!this.selectedContentType) {
                return null;
            }
            var contentRepositoryDescriptors = this.contentRepositoryDescriptors || dependency.resolve("epi.cms.contentRepositoryDescriptors");

            var repositoryDescriptor,
                matchType = function (type) {
                    return TypeDescriptorManager.isBaseTypeIdentifier(this.selectedContentType, type);
                };
            for (var index in contentRepositoryDescriptors) {
                var descriptor = contentRepositoryDescriptors[index];
                if (array.some(descriptor.containedTypes, matchType, this)) {
                    repositoryDescriptor = descriptor;
                    break;
                }
            }

            console.log("descriptors", contentRepositoryDescriptors);
            console.log("preventContextualContentFor", repositoryDescriptor.preventContextualContentFor);

            return repositoryDescriptor.preventContextualContentFor;
        },

        _getTypesToDisplay: function () {
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
            //summary:
            //    Value's setter.
            //
            // value: String
            //    Value to be set.
            //
            // tags:
            //    protected

            //construct ContentReference object
            if (value) {
                this._contentRef = value;
                var contentLinkToSelect = this._contentRef === "-" ? null : this._contentRef;

                if (this.multiRootsMode) {
                    // We need to wait until tree load top level children and then fire onLoad() event
                    when(this.tree.onLoadDeferred, lang.hitch(this, function () {
                        this.tree.selectContent(contentLinkToSelect, true);
                    }));
                } else {
                    if (!epi.isEmpty(contentLinkToSelect)) {
                        this.tree.selectContent(contentLinkToSelect, true);
                    }
                }
            } else {
                this._contentRef = null;
                this._clearSelectedContent();
            }
        },

        _getValueAttr: function () {
            //summary:
            //    Value's getter
            // tags:
            //    protected

            this.inherited(arguments);

            if (this._contentRef) {
                return this._contentRef.toString();
            } else {
                return "";
            }
        },

        _setAllowedTypesAttr: function (value) {
            this._typesToDisplay = null;
            this._set("allowedTypes", value);
        },

        _clearSelectedContent: function () {
            // summary:
            //      Clear selected node(s) in tree
            // tags:
            //      private

            this.tree.set("selectedNodes", []);
            this.tree.set("selectedItems", []);
        },

        getActions: function () {
            // summary:
            //      Overridden from _ActionProvider to get the select current owner content action added to the containing widget
            //
            // returns:
            //      An array containing a select page action definition, if it is not a shared block

            return this.canSelectOwnerContent ? [
                {
                    name: "selectpage",
                    label: localization.currentpage,
                    settings: { type: "submit" },
                    action: lang.hitch(this, function () {
                        this._setValueOwnerContent();
                    })
                }
            ]
            : [];
        },

        _onTreeNodeClick: function (content) {
            console.log("treenodeClick");
            return false;
            //// summary:
            ////    Handle the inner tree's content change event.
            ////
            //// content: Object
            ////    The content object.
            ////
            //// tags:
            ////    private

            //var value = content.contentLink;

            //if (!this._checkAcceptance(content.typeIdentifier)) {
            //    this.tree.lastFocused.setSelected(false);
            //    value = null;
            //}

            //this.set("value", value);
            //this.onChange(this.get("value"));
        },

        _checkAcceptance: function (typeIdentifier) {
            // summary:
            //    Compares a type against arrays of allowed and restricted types
            //
            // typeIdentifier: String
            //    The type to check if it's accepted to use
            //
            // tags:
            //    protected

            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([typeIdentifier], this.allowedTypes, this.restrictedTypes);

            return !!acceptedTypes.length;

        },

        _setValueOwnerContent: function () {
            this.set("value", "-");
        },

        onChange: function (contentLink) {
            // summary:
            //    Fired when value is changed.
            //
            // contentLink:
            //    The content link
            // tags:
            //    public, callback
        },

        focus: function () {
            this.tree.focus();
        }
    });

});
