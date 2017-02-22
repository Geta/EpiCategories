define("geta-epicategories/contentediting/CategoryReferenceListEditor", [
    // dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/promise/all",
    "dojo/dom-attr",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dojo/query",


    // dijit
    "dijit/_TemplatedMixin",
    "dijit/_Widget",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/form/Button",
    "dijit/Tooltip",

    // epi
    "epi-cms/widget/_HasChildDialogMixin",
    "geta-epicategories/contentediting/CategorySelectorTreeDialog",
    "epi/shell/widget/_ValueRequiredMixin",
    "epi/dependency",
    "epi/epi",
    "epi/shell/widget/dialog/Dialog",
        "epi-cms/ApplicationSettings",

    "dojo/text!./templates/CategoryReferenceListEditor.html",
    "epi/i18n!epi/cms/nls/episerver.cms.widget.CategorySelector"
],

function (
    // dojo
    array,
    declare,
    lang,
    all,
    domAttr,
    domClass,
    domConstruct,
    domStyle,
    query,

    // dijit
    _TemplatedMixin,
    _Widget,
    _WidgetsInTemplateMixin,
    Button,
    Tooltip,

    // epi
    _HasChildDialogMixin,
    CategorySelectorTreeDialog,
    _ValueRequiredMixin,
    dependency,
    epi,
    Dialog,
     ApplicationSettings,

    template,
    res
) {
    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _HasChildDialogMixin, _ValueRequiredMixin], {
        templateString: template,

        localization: res,
        //this widget wants a value that is an array
        multiple: true,

        value: null,

        store: null,
        _categories: null,
        _categoriesParentsName: {},

        repositoryKey: "categories", // should be able to filter based on parameter?

        focus: function () {
            this.button.focus();
        },

        postMixInProperties: function () {
            // summary:
            //      Initialize properties
            // tags:
            //      protected

            this.inherited(arguments);


            //if (!this.store) {
            //    var registry = dependency.resolve("epi.storeregistry");
            //    this.store = registry.get("epicategories");
            //}

        },















        // opens the dialog with category selector
        _onButtonClick: function () {
            //summary:
            //    Handle add category button click
            // tags:
            //    private

            if (!this.dialog) {
                this._createDialog();
            }

            this.dialog.show(true);
        },



        // creates the dialog for the actual picker
        _createDialog: function () {
            // summary:
            //		Create page tree dialog
            // tags:
            //    protected

            this.categorySelectorDialog = new CategorySelectorTreeDialog({
                repositoryKey: this.repositoryKey

            });

            this.dialog = new Dialog({
                title: this.localization.popuptitle,
                content: this.categorySelectorDialog,
                destroyOnHide: false,
                dialogClass: "epi-dialog-portrait"
            });
            this.own(this.dialog);

            // attach events to the dialog
            this.connect(this.dialog, "onExecute", "_onExecute");
            this.connect(this.dialog, "onShow", "_onShow");
            this.connect(this.dialog, "onHide", "_onDialogHide");

            this.dialog.startup();
        },

        // when dialog is saved
        _onExecute: function () {
            //summary:
            //    Handle dialog close
            // tags:
            //    private

            var categoriesSelected = this.categorySelectorDialog.get("value");

            this.set("value", categoriesSelected);
        },

        // when dialog is opened
        _onShow: function () {
            //summary:
            //    Handle onShow dialog event.
            // tags:
            //    private

            // the original categories need to keep to support cancel case, so a clone of categories need to pass to category selector dialog.
            this.categorySelectorDialog.set("value", lang.clone(this._categories));
            this.categorySelectorDialog.set("selectedCategoriesData", this._getCategoriesParentsNameClone());

            this.isShowingChildDialog = true;
            this.categorySelectorDialog.onShow();
        },

        // when dialog close
        _onDialogHide: function () {
            //summary:
            //    Handle dialog close
            // tags:
            //    private
            this.focus();
            this.isShowingChildDialog = false;
        },










        _getCategoriesParentsNameClone: function () {
            //summary:
            //    get a clone of categoriesPath.
            // tags:
            //    private

            var categoriesParentsNameClone = {};
            array.forEach(this._categories, lang.hitch(this, function (categoryId) {
                categoriesParentsNameClone[categoryId] = this._categoriesParentsName[categoryId];
            }));

            return categoriesParentsNameClone;
        },





    });
});
