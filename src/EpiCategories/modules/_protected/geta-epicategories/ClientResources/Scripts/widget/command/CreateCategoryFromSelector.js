define("geta-epicategories/widget/command/CreateCategoryFromSelector", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/Evented",
    "dojo/aspect",
    "dojo/on",
    "dojo/topic",
    "dojo/when",

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
    Evented,
    aspect,
    on,
    topic,
    when,

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
        // tags:
        //      internal

        // canExecute: [Boolean]
        //      Default value indicate that command can execute
        canExecute: true,

        // canSelectOwnerContent:
        //      Indicate that command can select owner content
        canSelectOwnerContent: false,

        currentContent: null,

        // showButtons:
        //      Indicate that command show buttons
        showButtons: false,

        // value: [String]
        //      Default select content
        value: null,

        // showRoot: [Boolean]
        //      True if show root on dialog, otherwise don't show root
        showRoot: false,

        // roots: [Array]
        //      The roots to show in the selector.
        roots: null,

        // creatingTypeIdentifier: [String]
        //      Type identifier of the content which is being created.
        creatingTypeIdentifier: null,

        // containerTypeIdentifiers: [Array]
        //      Type identifiers of content
        containerTypeIdentifiers: null,

        // modelContent: [Widget]
        //      That model selector dialog
        modelContent: null,

        // confirmActionText: [String]
        //      Confirm action text in dialog
        confirmActionText: createcontentselectorresources.buttons.confirmation,

        // description: [String]
        //      Description in dialog
        description: null,

        // title: [String]
        //      Title in dialog
        title: null,

        // autoPublish: Boolean
        //     Indicates if the content should be published automatically when created if the user has publish rights.
        autoPublish: true,

        typeDescriptorManager: null,

        contentRepositoryDescriptors: null,

        // allowedTypes: [public] Array
        //      The types which are allowed for the given property. i.e used for filtering based on AllowedTypesAttribute
        allowedTypes: null,

        // restrictedTypes: [public] Array
        //      The types which are restricted.
        restrictedTypes: null,

        postscript: function () {
            // summary:
            //    Initial settings value.
            //
            // tags:
            //    public

            this.inherited(arguments);
            this._initialize();
        },

        _initialize: function () {
            // summary:
            //    Initial application setting
            //
            // tags:
            //    private
            if (!this.creatingTypeIdentifier) {
                throw "You need to specify a creatingTypeIdentifier";
            }

            if (this._initialized) {
                return;
            }
            var registry = dependency.resolve("epi.storeregistry");
            this.contentStore = this.contentStore || registry.get("epi.cms.content.light");
            this.contextService = this.contextService || dependency.resolve("epi.shell.ContextService");
            this.contentTypeStore = this.contentTypeStore || registry.get("epi.cms.contenttype");

            this.contentRepositoryDescriptors = this.contentRepositoryDescriptors || dependency.resolve("epi.cms.contentRepositoryDescriptors");

            var repositoryDescriptor,
                matchType = function (type) {
                    return type === this.creatingTypeIdentifier;
                };

            for (var index in this.contentRepositoryDescriptors) {
                var descriptor = this.contentRepositoryDescriptors[index];
                //use the first descriptor that matches the creating type identifier.
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

        _execute: function () {
            // summary:
            //		Executes this command; ...
            // tags:
            //		protected

            var currentContent = this.model;

            if (!currentContent) {
                return;
            }

            when(this._getAvailableContentTypes(currentContent.contentLink), lang.hitch(this, function (availableContentTypes) {
                var contentTypeCanBeChildToCurrentContext = this._isContentTypeAllowedAsChildToCurrentContext(availableContentTypes, currentContent);

                // Show location selector dialog when current context is a valid location to store the one we are about to create.
                if (contentTypeCanBeChildToCurrentContext) {
                    this._switchView(currentContent);
                    this.emit('onExecuted', { command: this });
                }
            }));
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

        _switchView: function (content) {
            // summary:
            //    Change view to Create Content with parent content
            //
            // tags:
            //    protected

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
