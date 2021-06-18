namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EventID { get; set; }

        public int TournamentID { get; set; }

        [ForeignKey("TournamentID")]
        public virtual Tournament Tournament { get; set; }

        public virtual ICollection<PlayerActivityMatch> PlayerActivityMatches { get; set; }
        public virtual ICollection<Match> Matches { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string EventDisplayKey { get; set; }

        [StringLength(300)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Venue { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short? Year { get; set; }

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

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string SubGroupCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string SubGroupDesc { get; set; }

        public byte? WeekNumber { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string Grade { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string GradeDesc { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string GroupName { get; set; }

        [StringLength(7)]
        [Column(TypeName = "VARCHAR")]
        public string EventClassificationCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string EventClassificationDesc { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string MatchTypeCode { get; set; }

        [StringLength(15)]
        [Column(TypeName = "VARCHAR")]
        public string MatchTypeDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerTypeCode { get; set; }

        [StringLength(10)]
        [Column(TypeName = "VARCHAR")]
        public string PlayerTypeDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string AgeCategoryCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string AgeCategoryDesc { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string DrawsheetStructureCode { get; set; }

        [StringLength(30)]
        [Column(TypeName = "VARCHAR")]
        public string DrawsheetStructureDesc { get; set; }

        public short? DrawsheetSize { get; set; }

        public short? Drawsize { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizeMoney { get; set; }

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
    }
}
