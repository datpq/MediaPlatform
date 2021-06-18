using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Cms
{
    public class ImageAssetViewModel
    {
        public int AssetId { get; set; }
        public ICollection<MediaAssetDescriptionViewModel> Descriptions { get; set; }
        public string Photographer { get; set; }
        public int PhotographerId { get; set; }
    }
}
