using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;

namespace Geta.EpiCategories
{
    [ContentType(GUID = "ef0422aa-e1f7-476e-b7f7-50d76db7201f")]
    public class CategoryData : StandardContentBase
    {
        [Display(Order = 10)]
        [Required]
        public virtual string SystemName { get; set; }

        [Display(Order = 20)]
        [UIHint(UIHint.LongString)]
        [CultureSpecific]
        public virtual string Description { get; set; }

        [Display(Order = 30)]
        [CultureSpecific]
        public virtual bool IsSelectable { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            IsSelectable = true;
        }
    }
}