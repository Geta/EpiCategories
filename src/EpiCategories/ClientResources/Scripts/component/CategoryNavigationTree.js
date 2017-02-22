define("geta-epicategories/component/CategoryNavigationTree", [
// dojo
    "dojo/_base/array",
    "dojo/_base/connect",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "epi-cms/component/ContentNavigationTree",
    "geta-epicategories/component/_CategoryTreeNode",

// resources
    "epi/i18n!epi/cms/nls/episerver.cms.components.createcategory",
    "epi/i18n!epi/cms/nls/episerver.cms.components.categorytree",
    "epi/i18n!epi/cms/nls/episerver.shared.header"
],

function (
// dojo
    array,
    connect,
    declare,
    lang,

    ContentNavigationTree,
    _CategoryTreeNode,
// resources
    resCreateCategory,
    res
) {

    return declare([ContentNavigationTree], {
        res: res,
        selectionMode: false,

        // change the default nodetype to our custom
        nodeConstructor: _CategoryTreeNode,
        _selectedValues: [],
        _nodes: [],

        postMixInProperties: function () {
            this.inherited(arguments);
        },

        _createTreeNode: function () {
            // override the params telling the node it is in selectionMode
            lang.mixin(arguments[0], {
                'selectionMode': this.selectionMode
            });

            // construct the treenode (_CategoryTreeNode)
            var treeNode = this.inherited(arguments);

            // listen for when checkbox is changed
            this.connect(treeNode, "onCheckboxChanged", "onCheckboxChanged");

            // append the node to an array, to be able to calculate values when checked items change
            this._nodes.push(treeNode);
            return treeNode;
        },

        _calculateChecked: function () {
            // clear the selected values
            this._selectedValues = [];

            // iterate the appended nodes and see if they are selected
            var scope = this;
            this._nodes.forEach(function (node) {
                if (node.isChecked()) {
                    scope._selectedValues.push(node.item.contentLink);
                }
            }, this);

            console.log("selected values is", this._selectedValues);
        },

        onCheckboxChanged: function () {
            // trigger recalculate of checked items
            this._calculateChecked();
        },


    });
});
