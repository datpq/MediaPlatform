namespace ITF.DataServices.SDK.Models.Cms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ITFRelationships
    {
        [Key]
        public int relationshipId { get; set; }

        public int? assetId { get; set; }

        public short? assetTypeId { get; set; }

        public int? relatedAssetId { get; set; }

        public short? relatedAssetTypeId { get; set; }

        public DateTime? createDate { get; set; }

        public int? creatorUserId { get; set; }

        [StringLength(50)]
        public string notes { get; set; }

        public int? sortOrder { get; set; }

        [ForeignKey("assetTypeId")]
        public virtual ITFAssetType AssetType { get; set; }

        [ForeignKey("relatedAssetTypeId")]
        public virtual ITFAssetType RelatedAssetType { get; set; }

        [ForeignKey("relatedAssetId")]
        public virtual CmsPropertyData CmsPropertyData { get; set; }
    }
}
