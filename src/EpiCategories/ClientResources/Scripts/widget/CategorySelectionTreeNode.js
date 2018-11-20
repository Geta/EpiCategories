define("geta-epicategories/widget/CategorySelectionTreeNode", [
// dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/event",
    "dojo/when",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/string",
    "dojo/keys",
    "dojo/Evented",
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
    event,
    when,
    domClass,
    domConstruct,
    string,
    keys,
    Evented,

// dijit
    CheckBox,
// epi
    _ContentTreeNode,
    dependency,
    epi,
// template
    templateString
) {

    return declare([_ContentTreeNode, Evented], {
        _checkbox: null,
        _contextMenuClass: "epi-iconContextMenu",
        _selected: false,

        checked: false,
        hasContextMenu: true,
        store: null,
        isSelectable: true,
        templateString: templateString,

        postCreate: function() {
            this.inherited(arguments);

            if (!this.store) {
                var registry = dependency.resolve('epi.storeregistry');
                this.store = registry.get('epi.cms.contentdata');
            }

            domClass.add(this.iconNode, "dijitHidden");

            if (this.isSelectable) {
                this._createCheckbox();
            }
        },

        setSelected: function (/*Boolean*/selected) {
            this.inherited(arguments);

            if (this.hasContextMenu) {
                domClass.toggle(this.iconNodeMenu, this._contextMenuClass, this._selected = selected);
            }
        },

        showContextMenu: function (/* Boolean */show) {
            if (!this._selected && this.hasContextMenu) {
                domClass.toggle(this.iconNodeMenu, this._contextMenuClass, show);
            }
        },

        _createCheckbox: function () {
            if (this.item.name === 'ROOT' || this.item.typeIdentifier === 'episerver.core.contentfolder') {
                return;
            }

            when(this.store.get(this.item.contentLink)).then(lang.hitch(this, function (content) {
                if (!content.properties.isSelectable) {
                    return;
                }

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

        _onClickIconNodeMenu: function (evt) {
            event.stop(evt);

            // Since we're stopping the event we're preventing the regular focus handling,
            // which means that the context menu won't have anything to restore focus to when closed
            this.labelNode.focus();
            this.emit("onContextMenuClick", { node: this, target: evt.target, x: evt.pageX, y: evt.pageY });
        },

        _onMouseDownIconNodeMenu: function (evt) {
            //Stop mouse down event when clicking on the context menu icon
            event.stop(evt);
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
