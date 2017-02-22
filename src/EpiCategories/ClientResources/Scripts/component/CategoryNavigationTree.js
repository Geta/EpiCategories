define("geta-epicategories/component/CategoryNavigationTree", [
// dojo
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "epi-cms/component/ContentNavigationTree",
// resources
    "epi/i18n!epi/cms/nls/episerver.cms.components.createcategory",
    "epi/i18n!epi/cms/nls/episerver.cms.components.categorytree",
    "epi/i18n!epi/cms/nls/episerver.shared.header"
],

function (
// dojo
    array,
    declare,
    lang,

    ContentNavigationTree,
// resources
    resCreateCategory,
    res
) {

    return declare([ContentNavigationTree], {
        res: res,

        postMixInProperties: function () {
            console.log("hejejjejej");
        },

    });
});
