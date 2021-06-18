using System;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class ITFHtml
    {
        public int Id { get; set; }
        public int? NodeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CultureCode { get; set; }
        public string Scope { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string HTML { get; set; }
    }
}
