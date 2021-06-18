namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NationActivityTie")]
    public partial class NationActivityTie
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TieID { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string TennisCategoryCode { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "VARCHAR")]
        [StringLength(3)]
       
        public string NationCode { get; set; }

        [StringLength(100)]
        public string NationName { get; set; }

        public short? NationSeeding { get; set; }

        [StringLength(3)]
        public string OpponentNationCode { get; set; }

        [StringLength(100)]
        public string OpponentNationName { get; set; }

        public short? OpponentNationSeeding { get; set; }

        [StringLength(1)]
        public string ResultCode { get; set; }

        public byte? WinCount { get; set; }

        public byte? LossCount { get; set; }

        [StringLength(50)]
        public string Score { get; set; }

        [StringLength(2)]
        public string PlayStatusCode { get; set; }

        [StringLength(3)]
        public string ResultStatusCode { get; set; }

        public byte? RoundNumber { get; set; }

        [StringLength(5)]
        public string RoundCode { get; set; }

        [StringLength(25)]
        public string RoundDesc { get; set; }

        [StringLength(5)]
        public string TourCategoryCode { get; set; }

        [StringLength(50)]
        public string TourCategoryDesc { get; set; }

        [StringLength(100)]
        public string Venue { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short? Year { get; set; }

        [StringLength(100)]
        public string TournamentName { get; set; }

        [StringLength(100)]
        public string ZonalName { get; set; }

        [StringLength(300)]
        public string EventName { get; set; }

        [StringLength(100)]
        public string TieName { get; set; }

        [StringLength(60)]
        public string TieResultShort { get; set; }

        [StringLength(350)]
        public string TieResultLong { get; set; }

        [StringLength(3)]
        public string HostNationCode { get; set; }

        [StringLength(100)]
        public string HostNationName { get; set; }

        [StringLength(1)]
        public string SurfaceCode { get; set; }

        [StringLength(25)]
        public string SurfaceDesc { get; set; }

        [StringLength(1)]
        public string IndoorOutdoorFlag { get; set; }

        [StringLength(3)]
        public string EventDivisionCode { get; set; }

        [StringLength(50)]
        public string EventDivisionDesc { get; set; }

        [StringLength(3)]
        public string EventZoneCode { get; set; }

        [StringLength(50)]
        public string EventZoneDesc { get; set; }

        [StringLength(1)]
        public string EventSubZoneCode { get; set; }

        [StringLength(50)]
        public string EventSubZoneDesc { get; set; }

        [StringLength(3)]
        public string EventSubGroupCode { get; set; }

        [StringLength(50)]
        public string EventSubGroupDesc { get; set; }

        [StringLength(7)]
        public string EventClassificationCode { get; set; }

        [StringLength(50)]
        public string EventClassificationDesc { get; set; }

        [StringLength(2)]
        public string EventDrawsheetStructureCode { get; set; }

        [StringLength(30)]
        public string EventDrawsheetStructureDesc { get; set; }

        [Required]
        [StringLength(100)]
        public string DisplayOrder { get; set; }

        [StringLength(100)]
        public string PublicTournamentId { get; set; }

        [StringLength(200)]
        public string PublicTieId { get; set; }
    }
}
