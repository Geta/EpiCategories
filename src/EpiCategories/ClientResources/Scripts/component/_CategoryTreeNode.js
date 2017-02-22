define("geta-epicategories/component/_CategoryTreeNode", [
// dojo
    "dojo/_base/declare",
       "dojo/_base/lang",
         "dojo/_base/connect",

    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/string",
      "dojo/on",


// dijit
    "dijit/Tree",
    "dijit/_TemplatedMixin",
        "dijit/form/CheckBox",
// dojox
    "dojox/html/ellipsis",
// epi
    "epi-cms/component/_ContentNavigationTreeNode",
    "epi/shell/widget/_ModelBindingMixin",
// template
    "dojo/text!./templates/_CategoryTreeNode.html"
],

function (
// dojo
    declare,
    lang,
    connect,
    domClass,
    domConstruct,
    string,
        on,

// dijit
    Tree,
    _TemplatedMixin,
        CheckBox,
// dojox
    htmlEllipsis,
// epi
    _ContentNavigationTreeNode,
    _ModelBindingMixin,
// template
    templateString
) {

    return declare([_ContentNavigationTreeNode, _ModelBindingMixin],
    {
        selectionMode: false,
        templateString: templateString,
        _checkbox: null,

        constructor: function (args) {
            if (args) {
                this.selectionMode = args.selectionMode;
            }
        },

        postCreate: function () {
            this.inherited(arguments);

            // check if tree is in selectionmode and insert checkboxes
            if (this.selectionMode) {
                this.createCheckbox();
            }
        },

        createCheckbox: function () {

            console.log("createCheckboxnode:", this);
            var isSelectable = true;

            if (isSelectable) {
                // Add the checkbox to the container.
                var container = domConstruct.create("div", { "class": "epi-checkboxContainer" }, this.domNode);

                // create the checkbox
                this._checkbox = new CheckBox({ value: this.item.contentLink /*item.value*/ });
                this._checkbox.placeAt(container);
                this.checkboxNode.appendChild(container);

                var scope = this;
                on(this._checkbox, "change", function () {
                    scope.onCheckboxChanged(this);
                });

                if (false)
                    this._checkbox.set("readOnly", !!this.readOnly);
            }

        },

        onCheckboxChanged: function () { },

        // check if current checkbox is checked.
        isChecked: function () {
            if (this._checkbox)
                return this._checkbox.checked;
            return false;
        },

        setChecked: function (value) {
            if (this._checkbox)
                this._checkbox.set('checked', value);
        }


    });
});