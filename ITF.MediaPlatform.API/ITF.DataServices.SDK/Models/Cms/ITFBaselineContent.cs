namespace ITF.DataServices.SDK.Models.Cms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ITFBaselineContent")]
    public partial class ITFBaselineContent
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int baselineId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short assetTypeId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(3)]
        public string websiteCode { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(2)]
        public string cultureCode { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string contentType { get; set; }

        [Column(TypeName = "ntext")]
        public string content { get; set; }

        [ForeignKey("assetTypeId")]
        public virtual ITFAssetType AssetType { get; set; }
    }
}
