define("geta-epicategories/widget/command/CreateCategoryFromSelector", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/Deferred",
    "dojo/Evented",
    "dojo/aspect",
    "dojo/on",
    "dojo/topic",
    "dojo/when",
    "dojo/promise/all",

// Shell
    "epi/dependency",
    "epi/shell/command/_Command",
    "epi/shell/TypeDescriptorManager",
    "epi/shell/widget/dialog/Dialog",

// CMS
    "epi-cms/core/ContentReference",
    "epi-cms/ApplicationSettings",
    "epi-cms/widget/ContentForestStoreModel",
    "epi-cms/contentediting/ContentActionSupport",
    "epi-cms/contentediting/_ContextualContentContextMixin",
    "epi/i18n!epi/cms/nls/episerver.cms.widget.createcontentselector"
],

function (
    array,
    declare,
    lang,
    Deferred,
    Evented,
    aspect,
    on,
    topic,
    when,
    all,

// Shell
    dependency,
    _Command,
    TypeDescriptorManager,
    Dialog,

// CMS
    ContentReference,
    ApplicationSettings,
    ContentForestStoreModel,
    ContentActionSupport,
    _ContextualContentContextMixin,
    createcontentselectorresources
) {

    return declare([_Command, Evented, _ContextualContentContextMixin], {
        canExecute: true,
        canSelectOwnerContent: false,
        showButtons: false,
        value: null,
        showRoot: false,
        roots: null,
        creatingTypeIdentifier: null,
        containerTypeIdentifiers: null,
        modelContent: null,
        confirmActionText: createcontentselectorresources.buttons.confirmation,
        description: null,
        title: null,
        autoPublish: true,
        typeDescriptorManager: null,
        contentRepositoryDescriptors: null,
        allowedTypes: null,
        restrictedTypes: null,

        postscript: function () {
            this.inherited(arguments);
            this._initialize();
        },

        _initialize: function () {
            if (!this.creatingTypeIdentifier) {
                throw "You need to specify a creatingTypeIdentifier";
            }

            if (this._initialized) {
                return;
            }

            var registry = dependency.resolve("epi.storeregistry");
            this.contentTypeStore = this.contentTypeStore || registry.get("epi.cms.contenttype");

            this.contentRepositoryDescriptors = this.contentRepositoryDescriptors || dependency.resolve("epi.cms.contentRepositoryDescriptors");

            var repositoryDescriptor,
                matchType = function (type) {
                    return type === this.creatingTypeIdentifier;
                };

            for (var index in this.contentRepositoryDescriptors) {
                var descriptor = this.contentRepositoryDescriptors[index];

                if (array.some(descriptor.containedTypes, matchType, this)) {
                    repositoryDescriptor = descriptor;
                    break;
                }
            }

            if (repositoryDescriptor) {
                var containerTypeIdentifiers = TypeDescriptorManager.getValue(this.creatingTypeIdentifier, "containerTypes");
                this.containerTypeIdentifiers = containerTypeIdentifiers ?
                    containerTypeIdentifiers :
                    [this.creatingTypeIdentifier];
                this.roots = this.roots || repositoryDescriptor.roots;
            }

            this.modelContent = new ContentForestStoreModel({
                roots: this.roots,
                additionalQueryOptions: {
                    sort: [{ attribute: "name", descending: false }]
                }
            });

            this.label = this.label || TypeDescriptorManager.getResourceValue(this.creatingTypeIdentifier, "create");
            this.title = this.title || TypeDescriptorManager.getResourceValue(this.creatingTypeIdentifier, "selectparent");
            this.description = this.description || TypeDescriptorManager.getResourceValue(this.creatingTypeIdentifier, "createdescription");
            this.iconClass = this.iconClass || TypeDescriptorManager.getValue(this.creatingTypeIdentifier, "commandIconClass");
            this._initialized = true;
        },

        _canExecute: function () {
            var model = this.model,
                createAction = ContentActionSupport.action.Create,
                createProviderCapability = ContentActionSupport.providerCapabilities.Create,
                canExecute = model && !(model.isWastebasket || model.isDeleted) &&  // Ensure a model is available and it isn't deleted
                    ContentActionSupport.hasLanguageAccess(model) &&  // Ensure the user has access in the current language.
                    ContentActionSupport.isActionAvailable(model, createAction, createProviderCapability, true);  // Ensure the action is available to the user.

            return !!canExecute;
        },
        
        _checkAvailability: function (availableContentTypes) {
            if (availableContentTypes.length === 0) {
                return false;
            }

            var deferred = new Deferred();

            var getRequests = array.map(availableContentTypes, lang.hitch(this, function (contentTypeID) {
                return this.contentTypeStore.get(contentTypeID);
            }));

            when(all(getRequests), lang.hitch(this, function (contentTypes) {
                var isAvailable = array.some(contentTypes, lang.hitch(this, function (contentType) {
                    return TypeDescriptorManager.isBaseTypeIdentifier(contentType.typeIdentifier, this.creatingTypeIdentifier);
                }));

                deferred.resolve(isAvailable);
            }));

            return deferred;
        },

        _execute: function () {
            var currentContent = this.model;

            if (!currentContent) {
                return;
            }

            when(this._getAvailableContentTypes(currentContent.contentLink), lang.hitch(this, function (availableContentTypes) {
                var contentTypeCanBeChildToCurrentContext = this._isContentTypeAllowedAsChildToCurrentContext(availableContentTypes, currentContent);

                // Switch to create content view.
                if (contentTypeCanBeChildToCurrentContext) {
                    this._switchView(currentContent);
                    this.emit('onExecuted', { command: this });
                }
            }));
        },

        _getContentType: function (model) {
            if (!model) {
                return null;
            }

            return this.contentTypeStore.get(model.contentTypeID);
        },

        _getAvailableContentTypes: function (contentLink) {
            return this.contentTypeStore.query({
                query: "getavailablecontenttypes",
                localAsset: false,
                parentReference: contentLink
            });
        },

        _isContentTypeAllowedAsChildToCurrentContext: function (availableContentTypes, currentContent) {
            var anyAvailableContentTypeOfCorrectType = array.some(availableContentTypes, function (contentType) {
                return TypeDescriptorManager.isBaseTypeIdentifier(contentType.typeIdentifier, this.creatingTypeIdentifier);
            }, this);

            var currentContentIsAValidContainerType = array.some(this.containerTypeIdentifiers, function (type) {
                return TypeDescriptorManager.isBaseTypeIdentifier(currentContent.typeIdentifier, type);
            });

            return anyAvailableContentTypeOfCorrectType && currentContentIsAValidContainerType;
        },

        _onModelChange: function () {
            if (this.model && this.model instanceof Array) {
                if (this.model.length === 1) {
                    this.model = this.model[0];
                } else { // model is an array that has more than one item, or empty.
                    this.model = null;
                }
            }

            this.set("canExecute", this._canExecute());

            var model = this.model;

            if (!model) {
                this.set("isAvailable", false);
                return;
            }

            when(this._getContentType(model), lang.hitch(this, function (contentType) {
                if (!contentType) {
                    this.set("isAvailable", false);
                    return;
                }

                when(this._checkAvailability(contentType.availableContentTypes), lang.hitch(this, function (isAvailable) {
                    this.set("isAvailable", isAvailable);
                }));
            }));
        },

        _switchView: function (content) {
            topic.publish("/epi/shell/action/changeview", "epi-cms/contentediting/CreateContent", null, {
                requestedType: this.creatingTypeIdentifier,
                parent: content,
                editAllPropertiesOnCreate: true,
                addToDestination: this.destination,
                createAsLocalAsset: false,
                treatAsSecondaryView: false,
                view: TypeDescriptorManager.getValue(this.creatingTypeIdentifier, "createView"),
                autoPublish: this.autoPublish,
                allowedTypes: this.allowedTypes,
                restrictedTypes: this.restrictedTypes
            });
        }
    });
});