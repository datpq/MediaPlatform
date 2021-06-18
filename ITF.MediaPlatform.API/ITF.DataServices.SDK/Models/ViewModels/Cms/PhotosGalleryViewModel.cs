using System;
using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Cms
{
    public class PhotosGalleryViewModel
    {
        public DateTime CreatedDate { get; set; }
        public int GalleryId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int PhotoId { get; set; }
        public string TitleEn { get; set; }
        public string TitleEs { get; set; }
    }

    public class CmsPhotoGalleriesViewModel
    {
        public int GalleryId { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int PhotoId { get; set; }
    }

    public class PhotoViewModel
    {
        public string PhotoId { get; set; }
        public string Desc { get; set; }
        public string Photographer { get; set; }
    }

    public class CmsPhotoGalleryViewModel
    {
        public int GalleryId { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public ICollection<PhotoViewModel> Photos { get; set; }
    }
}
