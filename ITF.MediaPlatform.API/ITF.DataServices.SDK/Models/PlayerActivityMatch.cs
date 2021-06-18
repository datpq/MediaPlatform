namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlayerActivityMatch")]
    public partial class PlayerActivityMatch
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MatchID { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public virtual Event Event { get; set; }

        public int? TieID { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string TennisCategoryCode { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "VARCHAR")]
        [StringLength(2)]
        public string MatchTypeCode { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerFamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerGivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerNationalityCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerNationalityName { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerPlayingHandCode { get; set; }

        [StringLength(3)]
        public string PlayerTieNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerTieNationName { get; set; }

        public short? PlayerSeeding { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerEntryClassificationCode { get; set; }

        public int? PartnerPlayerID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string PartnerPlayerFamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string PartnerPlayerGivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string PartnerPlayerNationalityCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PartnerPlayerNationalityName { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string PartnerPlayerPlayingHandCode { get; set; }

        public int? OpponentPlayerID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerFamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerGivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerNationalityCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerNationalityName { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerPlayingHandCode { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerTieNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerTieNationName { get; set; }

        public short? OpponentPlayerSeeding { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerEntryClassificationCode { get; set; }

        public int? OpponentPartnerPlayerID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPartnerPlayerFamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPartnerPlayerGivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPartnerPlayerNationalityCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPartnerPlayerNationalityName { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPartnerPlayerPlayingHandCode { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string ResultCode { get; set; }

        public byte? WinCount { get; set; }

        public byte? LossCount { get; set; }

        public byte? ScoreSet1Player { get; set; }

        public byte? ScoreSet1Opponent { get; set; }

        public byte? ScoreSet1LosingTieBreak { get; set; }

        public byte? ScoreSet2Player { get; set; }

        public byte? ScoreSet2Opponent { get; set; }

        public byte? ScoreSet2LosingTieBreak { get; set; }

        public byte? ScoreSet3Player { get; set; }

        public byte? ScoreSet3Opponent { get; set; }

        public byte? ScoreSet3LosingTieBreak { get; set; }

        public byte? ScoreSet4Player { get; set; }

        public byte? ScoreSet4Opponent { get; set; }

        public byte? ScoreSet4LosingTieBreak { get; set; }

        public byte? ScoreSet5Player { get; set; }

        public byte? ScoreSet5Opponent { get; set; }

        public byte? ScoreSet5LosingTieBreak { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string Score { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string PlayStatusCode { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string ResultStatusCode { get; set; }

        public byte? RoundNumber { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string RoundCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string RoundDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string IndoorOutdoorFlag { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string TourCategoryCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string TourCategoryDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationName { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Venue { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short? Year { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TournamentName { get; set; }

        [StringLength(125)]
        [Column(TypeName = "VARCHAR")]
        public string TournamentDetailName { get; set; }

        [StringLength(300)]
        [Column(TypeName = "VARCHAR")]
        public string EventName { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TieName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string EventDivisionCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string EventDivisionDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string EventZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string EventZoneDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string EventSubZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string EventSubZoneDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string EventSubGroupCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string EventSubGroupDesc { get; set; }

        [StringLength(7)]
        [Column(TypeName = "VARCHAR")]
        public string EventClassificationCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string EventClassificationDesc { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string EventDrawsheetStructureCode { get; set; }

        [StringLength(30)]
        [Column(TypeName = "VARCHAR")]
        public string EventDrawsheetStructureDesc { get; set; }

        [StringLength(60)]
        [Column(TypeName = "VARCHAR")]
        public string TieResultShort { get; set; }

        [StringLength(350)]
        [Column(TypeName = "VARCHAR")]
        public string TieResultLong { get; set; }

        public byte? TieRoundNumber { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string TieRoundCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string TieRoundDesc { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string DisplayOrder { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ReverseScore { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentId { get; set; }

        [StringLength(200)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTieId { get; set; }

        public int? DataExchangePlayerId { get; set; }

        public int? PartnerDataExchangePlayerId { get; set; }

        public int? OpponentDataExchangePlayerId { get; set; }

        public int? OpponentPartnerDataExchangePlayerId { get; set; }

        public bool? IsMatchScoreDiscrepancy { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerGender { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerEntryClassificationDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string PartnerPlayerGender { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerGender { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPlayerEntryClassificationDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string OpponentPartnerPlayerGender { get; set; }
    }
}
