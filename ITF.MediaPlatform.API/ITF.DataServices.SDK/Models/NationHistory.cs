namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NationHistory")]
    public partial class NationHistory
    {
        [Key]
        [Column(Order = 0, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string NationCode { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string HistoricNationCode { get; set; }
    }
}
