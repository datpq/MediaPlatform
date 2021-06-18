namespace ITF.DataServices.SDK.Models.Baseline02
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MatchDoubles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MatchDoublesID { get; set; }

        public int? EventID { get; set; }

        public int? TieID { get; set; }

        public int? Side1MatchPlayer1ID { get; set; }

        public int? Side1MatchPlayer2ID { get; set; }

        [StringLength(3)]
        public string Side1TieNationCode { get; set; }

        public int? Side2MatchPlayer1ID { get; set; }

        public int? Side2MatchPlayer2ID { get; set; }

        [StringLength(3)]
        public string Side2TieNationCode { get; set; }

        [StringLength(5)]
        public string RoundCode { get; set; }

        public byte? RoundNumber { get; set; }

        public short? DrawsheetPositionMatch { get; set; }

        public short? DrawsheetPositionSide1Player1 { get; set; }

        public short? DrawsheetPositionSide1Player2 { get; set; }

        public short? DrawsheetPositionSide2Player1 { get; set; }

        public short? DrawsheetPositionSide2Player2 { get; set; }

        [StringLength(3)]
        public string MatchScoringFormatTypeCode { get; set; }

        public byte? Side1Set1Score { get; set; }

        public byte? Side1Set1TieBreakScore { get; set; }

        public byte? Side2Set1Score { get; set; }

        public byte? Side2Set1TieBreakScore { get; set; }

        public byte? LosingSet1TieBreakScore { get; set; }

        public byte? Side1Set2Score { get; set; }

        public byte? Side1Set2TieBreakScore { get; set; }

        public byte? Side2Set2Score { get; set; }

        public byte? Side2Set2TieBreakScore { get; set; }

        public byte? LosingSet2TieBreakScore { get; set; }

        public byte? Side1Set3Score { get; set; }

        public byte? Side1Set3TieBreakScore { get; set; }

        public byte? Side2Set3Score { get; set; }

        public byte? Side2Set3TieBreakScore { get; set; }

        public byte? LosingSet3TieBreakScore { get; set; }

        public byte? Side1Set4Score { get; set; }

        public byte? Side1Set4TieBreakScore { get; set; }

        public byte? Side2Set4Score { get; set; }

        public byte? Side2Set4TieBreakScore { get; set; }

        public byte? LosingSet4TieBreakScore { get; set; }

        public byte? Side1Set5Score { get; set; }

        public byte? Side1Set5TieBreakScore { get; set; }

        public byte? Side2Set5Score { get; set; }

        public byte? Side2Set5TieBreakScore { get; set; }

        public byte? LosingSet5TieBreakScore { get; set; }

        [StringLength(1)]
        public string Side1Player1NoPointAllocationFlag { get; set; }

        [StringLength(1)]
        public string Side1Player2NoPointAllocationFlag { get; set; }

        [StringLength(1)]
        public string Side2Player1NoPointAllocationFlag { get; set; }

        [StringLength(1)]
        public string Side2Player2NoPointAllocationFlag { get; set; }

        public decimal? Side1Player1RoundPoints { get; set; }

        public decimal? Side1Player1BonusPoints { get; set; }

        public decimal? Side1Player2RoundPoints { get; set; }

        public decimal? Side1Player2BonusPoints { get; set; }

        public decimal? Side2Player1RoundPoints { get; set; }

        public decimal? Side2Player1BonusPoints { get; set; }

        public decimal? Side2Player2RoundPoints { get; set; }

        public decimal? Side2Player2BonusPoints { get; set; }

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

        [StringLength(1)]
        public string LiveRubberFlag { get; set; }

        public bool? IsLiveScoringProvided { get; set; }

        public int? LiveScoreboardCurrentMatchStatusId { get; set; }
    }
}
