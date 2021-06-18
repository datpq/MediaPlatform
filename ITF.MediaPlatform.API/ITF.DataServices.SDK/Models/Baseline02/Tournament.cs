namespace ITF.DataServices.SDK.Models.Baseline02
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

        [StringLength(100)]
        public string TournamentKey { get; set; }

        [StringLength(2)]
        public string TourSequenceCode { get; set; }

        [StringLength(1)]
        public string TourSubSequenceCode { get; set; }

        public int? TournamentGroupID { get; set; }

        [Required]
        [StringLength(3)]
        public string TennisCategoryCode { get; set; }

        [StringLength(12)]
        public string ExternalCode { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string MediaName { get; set; }

        [StringLength(100)]
        public string MediaVenue { get; set; }

        [StringLength(3)]
        public string HostNationCode { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short? TournamentYear { get; set; }

        [StringLength(5)]
        public string TourCategoryCode { get; set; }

        [StringLength(5)]
        public string MediaTourCategory { get; set; }

        [StringLength(5)]
        public string Grade { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizeMoney { get; set; }

        public int? PrizeMoneyID { get; set; }

        [StringLength(1)]
        public string HospitalityPaidFlag { get; set; }

        [StringLength(1)]
        public string DefaultSurfaceCode { get; set; }

        public int? DefaultSurfaceTypeID { get; set; }

        [StringLength(1)]
        public string DefaultIndoorOutdoorCode { get; set; }

        public byte? NumberOfCourts { get; set; }

        public int? DefaultBallID { get; set; }

        [StringLength(1)]
        public string LargerBallsFlag { get; set; }

        [StringLength(3)]
        public string DefaultMatchScoringFormatTypeCode { get; set; }

        public DateTime? RankCalcDate { get; set; }

        public DateTime? RankReportingDate { get; set; }

        public int? TournamentNumber { get; set; }

        public decimal? RankPointConversionCutOffSingles { get; set; }

        public decimal? RankPointConversionCutOffDoubles { get; set; }

        [StringLength(4)]
        public string EntryControlledByTennisBodyCode { get; set; }

        public DateTime? EntryDeadlineNonEntryListDate { get; set; }

        [StringLength(2)]
        public string TourStatusCode { get; set; }

        [StringLength(255)]
        public string TourStatusNote { get; set; }

        public DateTime? PostponedToDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? SanctionFee { get; set; }

        public DateTime? SanctionFeeDueDate { get; set; }

        [StringLength(1)]
        public string SanctionFeePaidFlag { get; set; }

        [Required]
        [StringLength(1)]
        public string PublishedCalendarFlag { get; set; }

        [Required]
        [StringLength(1)]
        public string PublishedFactsheetFlag { get; set; }

        [StringLength(1)]
        public string CertifiedFlag { get; set; }

        [Required]
        [StringLength(1)]
        public string SubmittedFlag { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(20)]
        public string LastUpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] TStamp { get; set; }

        public DateTime? EntryFreezeDeadlineDateAndTime { get; set; }

        [StringLength(1)]
        public string EntryFreezeDeadlineBlackOutLapsedFlag { get; set; }

        public DateTime? EntryTournamentOrganisersWindowOpenDateAndTime { get; set; }

        public DateTime? EntryTournamentOrganisersWindowCloseDateAndTime { get; set; }

        [StringLength(1)]
        public string EntryTournamentOrganisersWindowBlackOutLapsedFlag { get; set; }

        public int? TournamentEntryCategoryID { get; set; }

        [Required]
        [StringLength(1)]
        public string CodeOfConductBlackOutLapsedFlag { get; set; }

        public decimal? LocalTimeZone { get; set; }

        public int? LocalTimeZoneLocationID { get; set; }

        public bool? IsLiveScoringProvided { get; set; }

        public DateTime? LiveScoringStartDateAndTime { get; set; }

        public DateTime? LiveScoringDetailsLastUpdatedDateAndTime { get; set; }

        public int? OrderOfPlayId { get; set; }

        public bool? IsTmsDownloadRestrictionToBeIgnored { get; set; }

        public bool? IsTmsUploadRestrictionToBeIgnored { get; set; }
    }
}
