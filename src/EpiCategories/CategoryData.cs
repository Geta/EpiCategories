using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Geta.EpiCategories
{
    [ContentType(GUID = "ef0422aa-e1f7-476e-b7f7-50d76db7201f")]
    public class CategoryData : StandardContentBase, IRoutable
    {
        [UIHint(UIHint.PreviewableText)]
        [ScaffoldColumn(false)]
        [CultureSpecific]
        public virtual string URLSegment { get; set; }

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

        string IRoutable.RouteSegment
        {
            get { return URLSegment; }
            set { URLSegment = value; }
        }
    }
}