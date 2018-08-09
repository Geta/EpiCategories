define("geta-epicategories/component/CategoryNavigationTree", [
// dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "epi-cms/component/ContentNavigationTree",
    "geta-epicategories/widget/CategoryForestStoreModel"
],

function (
// dojo
    array,
    declare,
    lang,

    ContentNavigationTree,
    CategoryForestStoreModel
) {

    return declare([ContentNavigationTree], {
        _createTreeModel: function () {
            return new CategoryForestStoreModel({
                categorySettings: this.settings.categorySettings,
                roots: this.roots,
                typeIdentifiers: this.typeIdentifiers
            });
        }
    });
});
