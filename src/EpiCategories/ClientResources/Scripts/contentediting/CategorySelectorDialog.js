define("geta-epicategories/contentediting/CategorySelectorDialog", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
      "dojo/dom-geometry",
    "dojo/dom-class",
    "dojo/when",

    "dijit/layout/_LayoutWidget",
    "dijit/_TemplatedMixin",
    "dijit/_Widget",
    "dijit/_WidgetsInTemplateMixin",

    "dijit/form/Button",

    "epi",
    "epi/dependency",
     "dojo/text!./templates/CategorySelectorDialog.html",
    "geta-epicategories/component/CategoryNavigationTree"
],

function (
    array,
    declare,
    lang,
    domGeometry,
    domClass,
    when,

    _LayoutWidget,
    _TemplatedMixin,
    _Widget,
    _WidgetsInTemplateMixin,

    Button,

    epi,
    dependency,
    template,
    CategoryNavigationTree
) {
    /// byt ut denna mot en MainNavigationLiknande implementation av trädet?
    return declare([_LayoutWidget, _Widget, _TemplatedMixin, _WidgetsInTemplateMixin], {

        contentRepositoryDescriptors: null,

        postCreate: function () {

            console.log("postcreate!");


            this.contentRepositoryDescriptors = this.contentRepositoryDescriptors || dependency.resolve("epi.cms.contentRepositoryDescriptors");
            var settings = this.contentRepositoryDescriptors[this.repositoryKey];
            var roots = this.roots ? this.roots : settings.roots;

            console.log("settings: ",
                settings);


            var componentType = "geta-epicategories/component/CategoryNavigationTree";

            require([componentType], lang.hitch(this, function (innerComponentClass) {
                var innerComponent = new innerComponentClass(
                    {
                        typeIdentifiers: settings.mainNavigationTypes ? settings.mainNavigationTypes : settings.containedTypes,
                        containedTypes: settings.containedTypes,
                        settings: settings,
                        roots: roots,
                        repositoryKey: this.repositoryKey
                    }
                );
                innerComponent.placeAt(this.navigation);
                this.tree = innerComponent;

                //when(this.tree._isSiteMultilingual()).then(function (isSiteMultilingual) {
                //    if (!isSiteMultilingual) {
                //        return;
                //    }
                //    this.loadShowAllLanguages().then(function (showAllLanguages) {
                //        innerComponent.set("showAllLanguages", showAllLanguages);
                //        this._setShowAllLanguagesLinkVisibility(showAllLanguages);
                //    }.bind(this));
                //}.bind(this));

                // Resize the tree once it is added to the UI to ensure the indentation
                // is calculated correctly.
                this.tree.resize();

                //this.own(this.tree.watch("showAllLanguages", function (value) {
                //    var showAllLanguages = this.tree.get("showAllLanguages");
                //    this.search.set("filterOnCulture", !showAllLanguages);
                //    if (this.showAllLanguages !== showAllLanguages) {
                //        this.saveShowAllLanguages(showAllLanguages);
                //    }
                //    this.set("showAllLanguages", showAllLanguages);

                //    this._setShowAllLanguagesLinkVisibility(showAllLanguages);
                //}.bind(this)));
            }));


        },
        // summary:
        //      Category selector widget.
        //      Used for editing PropertyCategory properties in flyout editor.
        // tags:
        //      internal

        // templateString: [protected] String
        //      Widget's template string.
        templateString: template,


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
            console.log("onshow");
            // summary:
            //      handles onShow dialog event.
            // tags:
            //      public

            if (!this._selectedCategoryIds && !this.categoryTree) {
                return;
            }

            //this.categoryTree.set("selectedNodeIds", this._selectedCategoryIds);
            //this.categoryTree.set("selectedNodeData", this.selectedCategoriesData);
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
