define("geta-epicategories/widget/CategorySelectionTreeNode", [
// dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/when",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/string",
    "dojo/keys",

// dijit
    "dijit/form/CheckBox",
// epi
    "epi-cms/widget/_ContentTreeNode",
    "epi/dependency",
    "epi/epi",
// template
    "dojo/text!./templates/CategorySelectionTreeNode.html"
],

function (
// dojo
    declare,
    lang,
    when,
    domClass,
    domConstruct,
    string,
    keys,

// dijit
    CheckBox,
// epi
    _ContentTreeNode,
    dependency,
    epi,
// template
    templateString
) {

    return declare([_ContentTreeNode], {
        _checkbox: null,

        checked: false,
        store: null,
        templateString: templateString,

        postCreate: function() {
            this.inherited(arguments);

            if (!this.store) {
                var registry = dependency.resolve('epi.storeregistry');
                this.store = registry.get('epi.cms.contentdata');
            }

            this._createCheckbox();
        },

        _createCheckbox: function () {
            if (this.item.name === 'ROOT' || this.item.typeIdentifier === 'episerver.core.contentfolder') {
                return;
            }

            when(this.store.get(this.item.contentLink)).then(lang.hitch(this, function (content) {
                if (!content.properties.isSelectable) {
                    return;
                }

                domClass.add(this.iconNode, "dijitHidden");

                var container = domConstruct.create('span', {
                    'class': 'epi-checkboxNode dijitTreeExpando'
                });

                this._checkbox = new CheckBox({
                    name: "checkboxCategory",
                    value: this.item.contentLink,
                    tabIndex: -1,
                    checked: this.checked,
                    onChange: lang.hitch(this, function (checked) {
                        this.onNodeSelectChanged(checked, content);
                    })
                });

                this._checkbox.placeAt(container);
                domConstruct.place(container, this.expandoNode, "after");
            }));
        },

        _setCheckedAttr: function (checked) {
            this._set("checked", checked);

            if (!this._checkbox) {
                return;
            }

            this._checkbox.set("checked", checked);
        },

        _onLabelClick: function () {
            if (!this._checkbox) {
                return;
            }

            this.set("checked", !this._checkbox.checked);
        },

        _onLabelKeyUp: function (evt) {
            if (evt.keyCode === keys.SPACE || evt.keyCode === keys.ENTER) {
                this._onLabelClick();
            }
        },

        onNodeSelectChanged: function () {
        }
    });

});
