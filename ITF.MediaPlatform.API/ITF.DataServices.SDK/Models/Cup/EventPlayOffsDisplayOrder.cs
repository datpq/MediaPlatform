namespace ITF.DataServices.SDK.Models.Cup
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventPlayOffsDisplayOrder")]
    public partial class EventPlayOffsDisplayOrder
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string EventClassificationCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DisplayOrder { get; set; }
    }
}
