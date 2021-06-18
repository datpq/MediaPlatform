namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NationLongestWinningRunInTiesHeader")]
    public partial class NationLongestWinningRunInTiesHeader
    {
        [Key]
        [Column(Order = 0, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string NationName { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RunID { get; set; }

        public int TournamentID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short NumberOfTies { get; set; }

        [Required]
        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string RunOngoingFlag { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentId { get; set; }
    }
}
