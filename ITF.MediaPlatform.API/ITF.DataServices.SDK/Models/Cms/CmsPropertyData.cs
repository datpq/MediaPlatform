using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class CmsPropertyData
    {
        [ForeignKey("propertytypeid")]
        public virtual CmsPropertyType CmsPropertyType { get; set; }

        [Key]
        public int contentNodeId { get; set; }

        public DateTime? dataDate { get; set; }
        public int? dataInt { get; set; }
        public string dataNtext { get; set; }
        public string dataNvarchar { get; set; }
        public int id { get; set; }
        public int propertytypeid { get; set; }
        public Guid? versionId { get; set; }
    }
}
