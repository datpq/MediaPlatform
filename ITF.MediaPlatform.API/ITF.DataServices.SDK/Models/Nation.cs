namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Nation")]
    public partial class Nation
    {
        [Key]
        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string NationName { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string CurrentNationFlag { get; set; }
    }
}
