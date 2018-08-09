define("geta-epicategories/widget/CategoryForestStoreModel", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "epi/shell/TypeDescriptorManager",
    "epi-cms/widget/ContentForestStoreModel",
    "epi-cms/ApplicationSettings"
], function (
    array,
    declare,
    lang,
    TypeDescriptorManager,
    ContentForestStoreModel,
    ApplicationSettings
) {

    return declare([ContentForestStoreModel], {
        categorySettings: null,

        getObjectIconClass: function (/*Object*/item, /*String*/fallbackIconClass) {
            var defaultIconClass = TypeDescriptorManager.getValue(item.typeIdentifier, "iconClass");

            if (!defaultIconClass) {
                return fallbackIconClass;
            }

            var suffix = "";

            switch (parseInt(item.contentLink, 10)) {
                case ApplicationSettings.globalAssetsFolder:
                case this.categorySettings.globalCategoriesRoot:
                    suffix = "AllSites";
                    break;

                case ApplicationSettings.siteAssetsFolder:
                case this.categorySettings.siteCategoriesRoot:
                    suffix = "ThisSite";
                    break;

                default:
                    break;
            }

            return (suffix === "" && defaultIconClass === fallbackIconClass) ? defaultIconClass : (fallbackIconClass + " " + defaultIconClass + suffix);
        }
    });
});
