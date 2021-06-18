namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tie")]
    public partial class Tie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TieID { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TieDisplayKey { get; set; }

        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Venue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short? Year { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationName { get; set; }

        public byte? DrawsheetRoundNumber { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string DrawsheetRoundCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string DrawsheetRoundDesc { get; set; }

        public byte? DrawsheetPositionTie { get; set; }

        public short? DrawsheetPositionSide1 { get; set; }

        public short? DrawsheetPositionSide2 { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side1NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Side1NationName { get; set; }

        public int? Side1SquadID { get; set; }

        public short? Side1Seeding { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string Side1ChoiceOfGroundFlag { get; set; }

        public byte? Side1Score { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side2NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Side2NationName { get; set; }

        public int? Side2SquadID { get; set; }

        public short? Side2Seeding { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string Side2ChoiceOfGroundFlag { get; set; }

        public byte? Side2Score { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string ChoiceOfGroundDecidedByLotFlag { get; set; }

        public byte? HomeSide { get; set; }

        public byte? WinningSide { get; set; }

        [StringLength(60)]
        [Column(TypeName = "VARCHAR")]
        public string ResultShort { get; set; }

        [StringLength(350)]
        [Column(TypeName = "VARCHAR")]
        public string ResultLong { get; set; }

        [StringLength(10)]
        [Column(TypeName = "VARCHAR")]
        public string Score { get; set; }

        [StringLength(10)]
        [Column(TypeName = "VARCHAR")]
        public string ScoreReversed { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string PlayStatusCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string PlayStatusDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string ResultStatusCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ResultStatusDesc { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string DrawsheetStructureCode { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceDesc { get; set; }

        public int? SurfaceTypeID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceTypeDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string IndoorOutdoorFlag { get; set; }

        public int? BallID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string BallDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string BallSpeedCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string BallSpeedDesc { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string RefereeName { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string UmpireName1 { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string UmpireName2 { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string UmpireName3 { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string UmpireName4 { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string UmpireName5 { get; set; }

        [Column(TypeName = "text")]
        public string SiteNameAndAddress { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string SiteTel { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string SiteFax { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string SiteEmail { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string SiteWebsite { get; set; }

        [Column(TypeName = "text")]
        public string ContactNameAndAddress { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string ContactTel { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string ContactFax { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string ContactEmail { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string ContactWebsite { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TimeOfPlayDay1 { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TimeOfPlayDay2 { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TimeOfPlayDay3 { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string StadiumCapacity { get; set; }

        [StringLength(255)]
        [Column(TypeName = "VARCHAR")]
        public string TransportNearestAirport { get; set; }

        [Column(TypeName = "text")]
       public string TransportNote { get; set; }

        [Column(TypeName = "text")]
        public string TicketInfo { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string TicketFreeEntryFlag { get; set; }

        public DateTime? TicketSaleDate { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TicketHotlineTel { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TicketHotlineFax { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TicketEmailAddress { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TicketWebsite { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string TicketSoldOutFlag { get; set; }

        [Column(TypeName = "text")]
        public string MediaPreviewNote { get; set; }

        [StringLength(300)]
        [Column(TypeName = "VARCHAR")]
        public string EventName { get; set; }

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

        public short? EventDrawsheetSize { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string RubberInProgressFlag { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string LiveScoringOnWebFlag { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string FunctionOfficialDraw { get; set; }

        [StringLength(200)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTieId { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentId { get; set; }
    }
}
