using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class ITFMediaDescription
    {
        public string altText { get; set; }
        public DateTime? createDate { get; set; }

        public string cultureCode { get; set; }

        public DateTime? deleteDate { get; set; }

        public string description { get; set; }

        public string galleryCaption { get; set; }

        [Key]
        public int id { get; set; }

        [ForeignKey("umbracoMediaNodeId")]
        public virtual ITFRelationships ITFRelationships { get; set; }

        public DateTime? modifiedDate { get; set; }
        public string summary { get; set; }
        public string title { get; set; }
        public int? umbracoMediaNodeId { get; set; }
    }
}
