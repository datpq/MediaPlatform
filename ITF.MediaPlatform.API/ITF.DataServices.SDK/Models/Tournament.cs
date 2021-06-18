namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tournament")]
    public partial class Tournament
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TournamentID { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TournamentKey { get; set; }

        public int? TournamentGroupID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TournamentGroupDesc { get; set; }

        [Required]
        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string TennisCategoryCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string TournamentDisplayKey { get; set; }

        [StringLength(100)]
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

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string HostZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string HostZoneDesc { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short? Year { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string TourTypeFlag { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string TourCategoryCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string TourCategoryDesc { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string MediaTourCategory { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string Grade { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizeMoney { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string HospitalityPaidFlag { get; set; }

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

        public int? TournamentNumber { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string TourStatusCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string TourStatusDesc { get; set; }

        [Required]
        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string PublishedCalendarFlag { get; set; }

        [Column(TypeName = "text")]
        public string OverviewNote { get; set; }
    }
}
