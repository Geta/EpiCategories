define("geta-epicategories/contentediting/CategorySelectorTreeDialog", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dijit/_TemplatedMixin",
    "dijit/_Widget",
    "dijit/_WidgetsInTemplateMixin",

    "dijit/form/Button",

    "epi",
    "epi/dependency",
    "epi-cms/widget/CategoryTree"
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
    CategoryTree
) {

    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin], {
        // summary:
        //      Category selector widget.
        //      Used for editing PropertyCategory properties in flyout editor.
        // tags:
        //      internal

        // templateString: [protected] String
        //      Widget's template string.
        templateString: "<div class=\"epi-categorySelectorDialog\"> \
                            <div data-dojo-attach-point=\"categoryTree\" data-dojo-type=\"epi-cms/widget/CategoryTree\" data-dojo-props=\"rootCategory: ${rootCategory}\"></div> \
                        </div>",

        //templateString: "<div class=\"epi-categorySelectorDialog\"> \
        //                    <div data-dojo-attach-point=\"categoryTree\" data-dojo-type=\"epi-cms/widget/CategoryTree\"></div> \
        //                </div>",



        // selectedCategoriesData: [Object] public
        //      Object that contains necessary selected categories information
        selectedCategoriesData: null,

        // _selectedCategoryIds: [Array] private
        //      Array of selected category identities
        _selectedCategoryIds: null,

        rootCategory: null,

        onShow: function () {
            // summary:
            //      handles onShow dialog event.
            // tags:
            //      public

            if (!this._selectedCategoryIds && !this.categoryTree) {
                return;
            }

            this.categoryTree.set("selectedNodeIds", this._selectedCategoryIds);
            this.categoryTree.set("selectedNodeData", this.selectedCategoriesData);
        },

        _setValueAttr: function (value) {
            // summary:
            //      Value's setter.
            // value: String
            //      Value to be set.
            // tags:
            //      private

            this._selectedCategoryIds = value;

            if (!this._selectedCategoryIds) {
                this._selectedCategoryIds = [];
            }
        },

        _getValueAttr: function () {
            // summary:
            //      Value's getter
            // tags:
            //      private

            return this.categoryTree.get("selectedNodeIds");
        }
    });

});
