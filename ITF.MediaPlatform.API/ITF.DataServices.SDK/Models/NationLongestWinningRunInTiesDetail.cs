namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NationLongestWinningRunInTiesDetail")]
    public partial class NationLongestWinningRunInTiesDetail
    {
        [Required]
        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string NationName { get; set; }

        [Required]
        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string WinningNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string WinningNationName { get; set; }

        public byte? WinningNationScore { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string LosingOpponentNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string LosingOpponentNationName { get; set; }

        public byte? LosingOpponentNationScore { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RunID { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TieID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string DivisionCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string DivisionDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string ZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ZoneDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string SubZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string SubZoneDesc { get; set; }

        public byte? RoundNumber { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string RoundCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string RoundDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationName { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentId { get; set; }

        [StringLength(200)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTieId { get; set; }
    }
}
