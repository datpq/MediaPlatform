namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NationMostGamesInTieHeader")]
    public partial class NationMostGamesInTieHeader
    {
        [Key]
        [Column(Order = 0, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName="VARCHAR")]public string NationName { get; set; }

        public byte? NationScore { get; set; }

        [StringLength(1)]
        [Column(TypeName="VARCHAR")]public string WinCode { get; set; }

        [StringLength(3)]
        [Column(TypeName="VARCHAR")]public string OpponentNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName="VARCHAR")]public string OpponentNationName { get; set; }

        public byte? OpponentNationScore { get; set; }

        public byte BestOfRubbers { get; set; }

        [Required]
        [StringLength(1)]
        [Column(TypeName="VARCHAR")]public string SinceTieBreakIntroducedFlag { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TieID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short NumberOfGames { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Year { get; set; }

        [StringLength(3)]
        [Column(TypeName="VARCHAR")]public string DivisionCode { get; set; }

        [StringLength(50)]
        [Column(TypeName="VARCHAR")]public string DivisionDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName="VARCHAR")]public string ZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName="VARCHAR")]public string ZoneDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName="VARCHAR")]public string SubZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName="VARCHAR")]public string SubZoneDesc { get; set; }

        [StringLength(7)]
        [Column(TypeName="VARCHAR")]public string EventClassificationCode { get; set; }

        [StringLength(50)]
        [Column(TypeName="VARCHAR")]public string EventClassificationDesc { get; set; }

        public byte? RoundNumber { get; set; }

        [StringLength(5)]
        [Column(TypeName="VARCHAR")]public string RoundCode { get; set; }

        [StringLength(25)]
        [Column(TypeName="VARCHAR")]public string RoundDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName="VARCHAR")]public string HostNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName="VARCHAR")]public string HostNationName { get; set; }

        [StringLength(100)]
        [Column(TypeName="VARCHAR")]public string PublicTournamentId { get; set; }

        [StringLength(200)]
        [Column(TypeName="VARCHAR")]public string PublicTieId { get; set; }
    }
}
