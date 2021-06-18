namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventRound")]
    public partial class EventRound
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EventID { get; set; }

        public int TournamentID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string RoundDisplayKey { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short RoundNumber { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string RoundCode { get; set; }
    }
}
