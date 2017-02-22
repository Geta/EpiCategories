define("geta-epicategories/contentediting/CategorySelectorDialog", [
    // dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/dom-geometry",
    "dojo/dom-class",
    "dojo/when",

    // dijit
    "dijit/form/Button",

    //episerver
    "epi-cms/widget/ContentSelectorDialog",
    "epi",
    "epi/dependency"
],

function (
    // dojo
    array,
    declare,
    lang,
    domGeometry,
    domClass,
    when,

    // dijit
    Button,

    // episerver
    ContentSelectorDialog,
    epi,
    dependency
) {
    return declare([ContentSelectorDialog], {

        canSelectOwnerContent: false,
        showButtons: false,
        roots: null,
        allowedTypes: null,
        showAllLanguages: true,
        disableRestrictedTypes: true,

        postMixInProperties: function () {
            this.inherited(arguments);

            this.contentRepositoryDescriptors = this.contentRepositoryDescriptors || dependency.resolve("epi.cms.contentRepositoryDescriptors");
            var settings = this.contentRepositoryDescriptors[this.repositoryKey];

            this.roots = settings.roots;
            this.allowedTypes = settings.containedTypes;

            console.log("this: ", this);
            console.log("settings: ", settings);

            console.log("postmixin #32");
        }

    });

});
