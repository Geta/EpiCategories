define("geta-epicategories/widget/CategorySelectorDialog", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dijit/_TemplatedMixin",
    "dijit/_Widget",
    "dijit/_WidgetsInTemplateMixin",

    "dijit/form/Button",

    "epi",
    "epi/dependency",
    "geta-epicategories/widget/CategorySelectionTree"
],

function (
    array,
    declare,
    lang,

    _TemplatedMixin,
    _Widget,
    _WidgetsInTemplateMixin,

    Button,

    epi,
    dependency,
    CategorySelectionTree
) {

    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin], {

        selectedContentLinks: [],
        templateString: "<div class=\"epi-categorySelectorDialog\" data-dojo-attach-point=\"treeContainer\"> \
                         </div>",

        constructor: function() {
            this.inherited(arguments);
            this.selectedContentLinks = [];
        },

        buildRendering: function() {
            this.inherited(arguments);

            this.categoryTree = new CategorySelectionTree({
                roots: this.roots,
                allowedTypes: this.allowedTypes,
                typeIdentifiers: this.allowedTypes
            });

            this.categoryTree.placeAt(this.treeContainer);
        },

        onShow: function () {
            this.categoryTree.set('selectedContentLinks', this.selectedContentLinks);
        },

        _setValueAttr: function (value) {
            this.selectedContentLinks = value;

            if (!this.selectedContentLinks) {
                this.selectedContentLinks = [];
            }
        },

        _getValueAttr: function () {
            return this.categoryTree.get('selectedContentLinks');
        }
    });
});
