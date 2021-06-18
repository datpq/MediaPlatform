namespace ITF.DataServices.SDK.Models.Baseline02
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TournamentDetail")]
    public partial class TournamentDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TournamentDetailID { get; set; }

        public int TournamentID { get; set; }

        public byte? WeekNumber { get; set; }

        [StringLength(125)]
        public string Name { get; set; }

        [StringLength(125)]
        public string MediaName { get; set; }

        [StringLength(100)]
        public string MediaVenue { get; set; }

        [StringLength(3)]
        public string HostNationCode { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(5)]
        public string Grade { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizeMoney { get; set; }

        [StringLength(1)]
        public string HospitalityPaidFlag { get; set; }

        public decimal? GradeSub { get; set; }

        public decimal? GradeStars { get; set; }

        public int? GradeCompensation { get; set; }

        public decimal? GradeRating { get; set; }

        [StringLength(1)]
        public string DefaultSurfaceCode { get; set; }

        public int? DefaultSurfaceTypeID { get; set; }

        [StringLength(1)]
        public string DefaultIndoorOutdoorCode { get; set; }

        public byte? NumberOfCourts { get; set; }

        public int? DefaultBallID { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(20)]
        public string LastUpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] TStamp { get; set; }

        public DateTime? RankReportingDate { get; set; }

        public DateTime? RankCalcDate { get; set; }
    }
}
