namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventNation")]
    public partial class EventNation
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EventID { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string NationCode { get; set; }

        [StringLength(4)]
        [Column(TypeName = "VARCHAR")]
        public string FinalPositionCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string FinalPositionDesc { get; set; }
    }
}
