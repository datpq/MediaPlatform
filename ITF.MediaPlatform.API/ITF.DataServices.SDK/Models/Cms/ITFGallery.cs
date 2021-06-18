using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class ITFGallery
    {
        public DateTime CreatedDate { get; set; }
        public string DescriptionText { get; set; }
        public int Id { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? MediaTypeId { get; set; }
        public string Name { get; set; }

        [Key]
        public int? NodeId { get; set; }

        [Column("UpdatedDate.old")]
        public string UpdatedDate_old { get; set; }

        public virtual ICollection<ITFWebScope> ITFWebScopes { get; set; }
    }
}
