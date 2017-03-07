define("geta-epicategories/widget/CategorySelector", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/promise/all",
    "dojo/dom-attr",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dojo/query",
    "dojo/Deferred",

    "dijit/_TemplatedMixin",
    "dijit/_Widget",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/form/Button",
    "dijit/Tooltip",

    "epi-cms/widget/_HasChildDialogMixin",
    "geta-epicategories/widget/CategorySelectorDialog",
    "epi/shell/widget/_ValueRequiredMixin",
    "epi/dependency",
    "epi/epi",
    "epi/shell/widget/dialog/Dialog",

    "dojo/text!./templates/CategorySelector.html",
    "epi/i18n!epi/cms/nls/episerver.cms.widget.CategorySelector"
],

function (
    array,
    declare,
    lang,
    promiseAll,
    domAttr,
    domClass,
    domConstruct,
    domStyle,
    query,
    Deferred,

    _TemplatedMixin,
    _Widget,
    _WidgetsInTemplateMixin,
    Button,
    Tooltip,

    _HasChildDialogMixin,
    CategorySelectorDialog,
    _ValueRequiredMixin,
    dependency,
    epi,
    Dialog,

    template,
    res
) {

    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _HasChildDialogMixin, _ValueRequiredMixin], {
        _categories: null,
        _preventSetDialogValueOnShow: false,
        _updateDisplayPromise: null,

        categorySettings: null,
        allowedTypes: null,
        localization: res,
        repositoryKey: null,
        restrictedTypes: null,
        roots: null,
        searchArea: 'CMS/categories',
        settings: null,
        showSearchBox: true,
        store: null,
        templateString: template,
        value: null,

        onChange: function (value) {
        },

        postCreate: function () {
            if (!this.store) {
                var registry = dependency.resolve('epi.storeregistry');
                this.store = registry.get('epi.cms.content.light');
            }
        },

        destroy: function () {
            this.inherited(arguments);

            if (this._updateDisplayPromise) {
                this._updateDisplayPromise.cancel();
                this._updateDisplayPromise = null;
            }
        },

        destroyDescendants: function () {
            if (this._tooltip) {
                this._tooltip.destroy();
                delete this._tooltip;
            }

            this.inherited(arguments);
        },

        onCreateCategoryCommandExecuted: function () {
            this.dialog.hide();
            this._preventSetDialogValueOnShow = true;
        },

        onNewCategoryCreated: function (category) {
            this.dialog.show(true);
            this.categorySelectorDialog.appendValue(category.contentLink);
            this._preventSetDialogValueOnShow = false;
        },

        _setValueAttr: function (value) {
            if (!lang.isArray(value)) {
                value = [value];
            }

            var filteredValue = array.filter(value, function (v) {
                return !!v;
            });

            this._setValueAndFireOnChange(filteredValue);

            if (filteredValue.length > 0) {
                this._set("value", filteredValue);
            } else {
                this._set("value", null);
            }

            this._started && this.validate();
        },

        _setValueAndFireOnChange: function (value) {
            // Compare arrays
            if (epi.areEqual(this._categories, value)) {
                return;
            }

            this._categories = value;
            this.onChange(this._categories);
            this._updateDisplayNode();
        },

        _getValueAttr: function () {
            return this._categories;
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);

            domStyle.set(this.button.domNode, "display", value ? "none" : "");
            domClass.toggle(this.domNode, "dijitReadOnly", value);
        },

        focus: function () {
            this.button.focus();
        },

        _createCategoryButton: function (category) {
            //
            // TODO: create a widget for item with a template instead of creating dom nodes
            //

            //don't add a button if it's already added.
            if (query("div[data-epi-category-id=" + category.contentLink + "]", this.categoriesGroupContainer).length !== 0) {
                return;
            }

            var containerDiv = domConstruct.create("div", { "class": "dijitReset dijitLeft dijitInputField dijitInputContainer epi-categoryButton" });
            var buttonWrapperDiv = domConstruct.create("div", { "class": "dijitInline epi-resourceName" });
            var categoryNameDiv = domConstruct.create("div", { "class": "dojoxEllipsis", innerHTML: category.name });
            domConstruct.place(categoryNameDiv, buttonWrapperDiv);

            domConstruct.place(buttonWrapperDiv, containerDiv);

            // create tooltip for the div
            this._tooltip = new Tooltip({
                connectId: categoryNameDiv,
                label: category.name
            });

            var removeButtonDiv = domConstruct.create("div", { "class": "epi-removeButton", innerHTML: "&nbsp;" });
            domAttr.set(removeButtonDiv, "data-epi-category-id", category.contentLink);
            var eventName = removeButtonDiv.onClick ? "onClick" : "onclick";

            if (!this.readOnly) {
                this.connect(removeButtonDiv, eventName, lang.hitch(this, this._onRemoveClick));
                domConstruct.place(removeButtonDiv, buttonWrapperDiv);
            } else {
                domConstruct.place(domConstruct.create("span", { innerHTML: "&nbsp;" }), buttonWrapperDiv);
            }

            domConstruct.place(containerDiv, this.categoriesGroupContainer);
        },

        _createNoCategoriesChosenSpan: function () {
            domConstruct.create("div", {
                innerHTML: this.localization.nocategorieschosen,
                "class": "epi-categoriesGroup__message"
            }, this.categoriesGroupContainer, "only");
        },

        _updateDisplayNode: function () {
            this.categoriesGroupContainer.innerHTML = "";

            if (!this._categories || this._categories.length === 0) {
                this._createNoCategoriesChosenSpan();
                return;
            }

            var dfdList = [];

            this._categories.forEach(function (categoryId) {
                dfdList.push(this.store.get(categoryId));
            }, this);

            this._updateDisplayPromise = promiseAll(dfdList).then(lang.hitch(this, function (categories) {
                // Clear the pointer to the promise since it is resolved.
                this._updateDisplayPromise = null;

                categories.forEach(function (category) {
                    if (category) {
                        this._createCategoryButton(category);
                    } else {
                        var categoryIndex = this._categories.indexOf(category.contentLink);

                        if (categoryIndex !== -1) {
                            this._categories.splice(categoryIndex, 1);
                        }
                    }

                    // Create no categories chosen span if haven't any category in list.
                    if (this._categories.length === 0) {
                        this._createNoCategoriesChosenSpan();
                    }
                }, this);
            }));
        },

        _onRemoveClick: function (arg) {
            var categoryId = domAttr.get(arg.target, "data-epi-category-id");
            var categoryIndex = this._categories.indexOf(categoryId);

            if (categoryIndex === -1) {
                return;
            }

            var remainingCategories = lang.clone(this._categories);
            remainingCategories.splice(categoryIndex, 1);
            this.set("value", remainingCategories);
        },

        _onShow: function () {
            if (!this._preventSetDialogValueOnShow) {
                this.categorySelectorDialog.set("value", lang.clone(this._categories));
            }

            this.isShowingChildDialog = true;
            this.categorySelectorDialog.onShow();
        },

        _onExecute: function () {
            var categoriesSelected = this.categorySelectorDialog.get("value");
            this.set("value", categoriesSelected);
        },

        _onDialogHide: function () {
            this.focus();
            this.isShowingChildDialog = false;
        },

        _createDialog: function () {
            this.categorySelectorDialog = new CategorySelectorDialog({
                categorySettings: this.categorySettings,
                repositoryKey: this.repositoryKey,
                roots: this.roots,
                allowedTypes: this.allowedTypes,
                restrictedTypes: this.restrictedTypes,
                showSearchBox: this.showSearchBox,
                searchArea: this.searchArea,
                settings: this.settings
            });

            this.categorySelectorDialog.on('onCreateCategoryCommandExecuted', lang.hitch(this, this.onCreateCategoryCommandExecuted));
            this.categorySelectorDialog.on('onNewCategoryCreated', lang.hitch(this, this.onNewCategoryCreated));

            this.dialog = new Dialog({
                title: this.localization.popuptitle,
                content: this.categorySelectorDialog,
                destroyOnHide: false,
                dialogClass: "epi-dialog-portrait"
            });

            this.own(this.dialog);
            this.connect(this.dialog, "onExecute", "_onExecute");
            this.connect(this.dialog, "onShow", "_onShow");
            this.connect(this.dialog, "onHide", "_onDialogHide");

            this.dialog.startup();
        },

        _onButtonClick: function () {
            if (!this.dialog) {
                this._createDialog();
            }

            this.dialog.show(true);
        }
    });
});
