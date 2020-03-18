using EPiServer.Personalization.VisitorGroups;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Geta.EpiCategories
{
    public class CategoryCriterionSettings : CriterionModelBase
    {
        [Required]
        [DojoWidget(SelectionFactoryType = typeof(EditorDescriptors.CategoryListing), AdditionalOptions = "{ selectOnClick: true }", LabelTranslationKey = "Category")]
        [DisplayName("Category")]
        public string CategoryId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [DojoWidget(DefaultValue = 0, AdditionalOptions = "{constraints: {min: 0}, selectOnClick: true}", LabelTranslationKey = "Viewed at least")]
        [DisplayName("Viewed at least")]
        public int ViewedTimes { get; set; }

        public override ICriterionModel Copy() => ShallowCopy();
    }
}