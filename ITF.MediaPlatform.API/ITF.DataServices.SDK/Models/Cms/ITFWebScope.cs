using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class ITFWebScope
    {
        public DateTime? createDate { get; set; }

        [Key]
        public int id { get; set; }

        [ForeignKey("nodeId")]
        public virtual ITFGallery ITFGallery { get; set; }

        public int? nodeId { get; set; }

        public int? webScopeNodeId { get; set; }
    }
}
