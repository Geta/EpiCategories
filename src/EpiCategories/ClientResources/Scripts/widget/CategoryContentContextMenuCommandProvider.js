define("geta-epicategories/widget/CategoryContentContextMenuCommandProvider", [
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/Evented",
    "dijit/Destroyable",
// epi
    "epi/shell/command/_CommandProviderMixin",
    "epi/shell/command/_SelectionCommandMixin",
    "epi/shell/selection",
// geta
    "geta-epicategories/widget/command/CreateCategoryFromSelector"
],

function (
    declare,
    lang,
    Evented,
    Destroyable,
// epi
    _CommandProviderMixin,
    _SelectionCommandMixin,
    Selection,
    CreateCategoryFromSelector
) {

    return declare([_CommandProviderMixin, Destroyable, Evented], {
        clipboardManager: null,
        creatingTypeIdentifier: null,
        treeModel: null,

        _settings: null,

        postscript: function () {
            this.inherited(arguments);
            this._updateCommands();
        },

        updateCommandModel: function (contentData) {
            this.get("commands").forEach(function (command) {
                if (!command.isInstanceOf(_SelectionCommandMixin)) {
                    command.set("model", contentData);
                }
            });

            this._settings.selection.set("data", [{ type: "epi.cms.contentdata", data: contentData }]);
        },

        _updateCommands: function () {
            this.get("commands").forEach(function (command) {
                if (typeof command.destroy === "function") {
                    command.destroy();
                }
            });

            //Create the commands
            this._settings = {
                category: "context",
                model: this.treeModel,
                clipboard: this.clipboardManager,
                selection: new Selection()
            };

            var commands = [];

            var createCommand = new CreateCategoryFromSelector({
                category: 'context',
                creatingTypeIdentifier: this.creatingTypeIdentifier,
                autoPublish: true,
                allowedTypes: this.allowedTypes,
                restrictedTypes: this.restrictedTypes
            });

            createCommand.set('destination', {
                save: lang.hitch(this, function(category) {
                    this.emit('onNewCategoryCreated', category);
                })
            });

            createCommand.on('onExecuted', lang.hitch(this, function(command) {
                this.emit('onCreateCategoryCommandExecuted', command);
            }));

            commands.push(createCommand);

            this.set("commands", commands);
        }
    });
});
