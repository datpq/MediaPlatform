using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Cms
{
    public class TieRelatedAssetsViewModel
    {
        public int InternalId { get; set; }
        public string PublicId { get; set; }
        public ICollection<CultureViewModel> Cultures { get; set; }
    }

    public class CultureViewModel
    {
        public string CultureCode { get; set; }
        public ICollection<ArticleViewModel> Articles { get; set; }
        public ICollection<ImageViewModel> Photos { get; set; }
        public string Report { get; set; }
        public string TV { get; set; }
        public string CourtPaceRating { get; set; }
    }

    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public int? ImageId { get; set; }
        public string Media { get; set; }
    }

    public class ImageViewModel
    {
        public int ImageId { get; set; }
        public string Desc { get; set; }
        public string Photographer { get; set; }
    }
}
