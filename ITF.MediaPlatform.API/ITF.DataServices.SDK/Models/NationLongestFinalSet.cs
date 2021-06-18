namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NationLongestFinalSet")]
    public partial class NationLongestFinalSet
    {
        [Key]
        [Column(Order = 0, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string NationName { get; set; }

        public byte? NationScore { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string WinCode { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentNationName { get; set; }

        public byte? OpponentNationScore { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MatchID { get; set; }

        public int TieID { get; set; }

        [Required]
        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string MatchTypeCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short NumberOfGames { get; set; }

        public int WinningPlayer1ID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string WinningPlayer1DisplayName { get; set; }

        public int? WinningPlayer2ID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string WinningPlayer2DisplayName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string WinningNationCode { get; set; }

        public short? WinningScore { get; set; }

        public int LosingPlayer1ID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string LosingPlayer1DisplayName { get; set; }

        public int? LosingPlayer2ID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string LosingPlayer2DisplayName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string LosingNationCode { get; set; }

        public short? LosingScore { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Year { get; set; }

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

        public int? WinningPlayer1DataExchangePlayerId { get; set; }

        public int? WinningPlayer2DataExchangePlayerId { get; set; }

        public int? LosingPlayer1DataExchangePlayerId { get; set; }

        public int? LosingPlayer2DataExchangePlayerId { get; set; }
    }
}
