namespace ITF.DataServices.SDK.Models.Baseline02
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MatchSingles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MatchSinglesID { get; set; }

        public int? EventID { get; set; }

        public int? TieID { get; set; }

        public int? MatchPlayer1ID { get; set; }

        [StringLength(3)]
        public string MatchPlayer1TieNationCode { get; set; }

        public int? MatchPlayer2ID { get; set; }

        [StringLength(3)]
        public string MatchPlayer2TieNationCode { get; set; }

        [StringLength(5)]
        public string RoundCode { get; set; }

        public byte? RoundNumber { get; set; }

        public byte? DrawsheetPositionMatch { get; set; }

        public short? DrawsheetPositionPlayer1 { get; set; }

        public short? DrawsheetPositionPlayer2 { get; set; }

        public byte? Player1Set1Score { get; set; }

        public byte? Player1Set1TieBreakScore { get; set; }

        public byte? Player2Set1Score { get; set; }

        public byte? Player2Set1TieBreakScore { get; set; }

        public byte? LosingSet1TieBreakScore { get; set; }

        public byte? Player1Set2Score { get; set; }

        public byte? Player1Set2TieBreakScore { get; set; }

        public byte? Player2Set2Score { get; set; }

        public byte? Player2Set2TieBreakScore { get; set; }

        public byte? LosingSet2TieBreakScore { get; set; }

        public byte? Player1Set3Score { get; set; }

        public byte? Player1Set3TieBreakScore { get; set; }

        public byte? Player2Set3Score { get; set; }

        public byte? Player2Set3TieBreakScore { get; set; }

        public byte? LosingSet3TieBreakScore { get; set; }

        public byte? Player1Set4Score { get; set; }

        public byte? Player1Set4TieBreakScore { get; set; }

        public byte? Player2Set4Score { get; set; }

        public byte? Player2Set4TieBreakScore { get; set; }

        public byte? LosingSet4TieBreakScore { get; set; }

        public byte? Player1Set5Score { get; set; }

        public byte? Player1Set5TieBreakScore { get; set; }

        public byte? Player2Set5Score { get; set; }

        public byte? Player2Set5TieBreakScore { get; set; }

        public byte? LosingSet5TieBreakScore { get; set; }

        public decimal? Player1RoundPoints { get; set; }

        public decimal? Player1BonusPoints { get; set; }

        public decimal? Player2RoundPoints { get; set; }

        public decimal? Player2BonusPoints { get; set; }

        public byte? WinningSide { get; set; }

        [StringLength(1)]
        public string ScoreKnownFlag { get; set; }

        [StringLength(2)]
        public string PlayStatusCode { get; set; }

        [StringLength(3)]
        public string ResultStatusCode { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(20)]
        public string LastUpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] TStamp { get; set; }

        [StringLength(3)]
        public string MatchScoringFormatTypeCode { get; set; }

        [StringLength(1)]
        public string Player1NoPointAllocationFlag { get; set; }

        [StringLength(1)]
        public string Player2NoPointAllocationFlag { get; set; }

        [StringLength(1)]
        public string LiveRubberFlag { get; set; }

        public bool? IsLiveScoringProvided { get; set; }

        public int? LiveScoreboardCurrentMatchStatusId { get; set; }
    }
}
