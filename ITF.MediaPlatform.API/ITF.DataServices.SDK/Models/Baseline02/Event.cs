namespace ITF.DataServices.SDK.Models.Baseline02
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            EventEntries = new HashSet<EventEntry>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EventID { get; set; }

        public int TournamentDetailID { get; set; }

        [StringLength(12)]
        public string ExternalCode { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        public string MediaName { get; set; }

        [StringLength(100)]
        public string MediaVenue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(2)]
        public string MatchTypeCode { get; set; }

        [StringLength(7)]
        public string EventClassificationCode { get; set; }

        [Required]
        [StringLength(2)]
        public string DrawsheetStructureCode { get; set; }

        public short? DrawsheetSize { get; set; }

        public short? MaxDrawsize { get; set; }

        [StringLength(1)]
        public string PlayerTypeCode { get; set; }

        [StringLength(3)]
        public string AgeCategoryCode { get; set; }

        [StringLength(3)]
        public string ZoneCode { get; set; }

        [StringLength(25)]
        public string GroupName { get; set; }

        [StringLength(1)]
        public string SurfaceCode { get; set; }

        public int? SurfaceTypeID { get; set; }

        [StringLength(1)]
        public string IndoorOutdoorCode { get; set; }

        public byte? NumberOfCourts { get; set; }

        public int? BallID { get; set; }

        [StringLength(100)]
        public string SiteTennisClubName { get; set; }

        [StringLength(100)]
        public string SiteAddLine1 { get; set; }

        [StringLength(100)]
        public string SiteAddLine2 { get; set; }

        [StringLength(100)]
        public string SiteAddLine3 { get; set; }

        [StringLength(100)]
        public string SiteAddLine4 { get; set; }

        [StringLength(100)]
        public string SiteAddLine5 { get; set; }

        [StringLength(50)]
        public string SiteAddCity { get; set; }

        [StringLength(50)]
        public string SiteAddState { get; set; }

        [StringLength(20)]
        public string SiteAddPostCode { get; set; }

        [StringLength(3)]
        public string SiteAddNationCode { get; set; }

        [StringLength(100)]
        public string SiteTel { get; set; }

        [StringLength(100)]
        public string SiteMobile { get; set; }

        [StringLength(100)]
        public string SiteFax { get; set; }

        [StringLength(100)]
        public string SiteEmail { get; set; }

        [StringLength(100)]
        public string SiteWebsite { get; set; }

        [StringLength(3)]
        public string DefaultMatchScoringFormatTypeCode { get; set; }

        [StringLength(1)]
        public string BestOfFiveSetFinalFlag { get; set; }

        [StringLength(50)]
        public string EventSignInDateAndTime { get; set; }

        [StringLength(50)]
        public string EventCutOff { get; set; }

        [StringLength(1)]
        public string WinLossFlag { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizeMoney { get; set; }

        [Column(TypeName = "text")]
        public string GeneralNote { get; set; }

        public DateTime? EntryDeadlineNonEntryListDate { get; set; }

        public DateTime? EventSignInGMTDateAndTime { get; set; }

        [StringLength(1)]
        public string AutomaticPointAllocationFlag { get; set; }

        [StringLength(1)]
        public string AllocatePointsBasedOnRoundRobinFlag { get; set; }

        [Required]
        [StringLength(1)]
        public string SubmittedFlag { get; set; }

        [StringLength(20)]
        public string LockedBy { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(20)]
        public string LastUpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] TStamp { get; set; }

        [StringLength(1)]
        public string NoAdvantageScoringSystemFlag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventEntry> EventEntries { get; set; }
    }
}
