namespace ITF.DataServices.SDK.Models.Cms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ITFContent")]
    public partial class ITFContent
    {
        public int nodeId { get; set; }

        public int? parentNodeId { get; set; }

        [StringLength(1000)]
        public string title { get; set; }

        public string summary { get; set; }

        [Column(TypeName = "ntext")]
        public string body { get; set; }

        public int? contentTypeId { get; set; }

        [StringLength(100)]
        public string contentTypeAlias { get; set; }

        public int? websiteRootNodeId { get; set; }

        [StringLength(100)]
        public string websiteRootName { get; set; }

        public int? webletRootNodeId { get; set; }

        [StringLength(100)]
        public string webletRootName { get; set; }

        [StringLength(2)]
        public string cultureCode { get; set; }

        public int? languageRootNodeId { get; set; }

        public bool? published { get; set; }

        public DateTime dateStamp { get; set; }

        [Key]
        public int uniqueid { get; set; }
    }
}
