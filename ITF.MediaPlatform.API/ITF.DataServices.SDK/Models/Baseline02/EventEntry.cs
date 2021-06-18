namespace ITF.DataServices.SDK.Models.Baseline02
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventEntry")]
    public partial class EventEntry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EventEntryID { get; set; }

        public int EventID { get; set; }

        [StringLength(150)]
        public string EntryName { get; set; }

        public DateTime? EntryRankDate { get; set; }

        public DateTime? EntryDeadlineDate { get; set; }

        public DateTime? WithdrawalDeadlineMainDrawDate { get; set; }

        [StringLength(1)]
        public string WithdrawalDeadlineMainDrawBlackOutLapsedFlag { get; set; }

        public DateTime? WithdrawalDeadlineQualifyingDate { get; set; }

        [StringLength(1)]
        public string WithdrawalDeadlineQualifyingBlackOutLapsedFlag { get; set; }

        public short? NumberOfMainDrawSlots { get; set; }

        public short? NumberOfQualifyingSlots { get; set; }

        [Required]
        [StringLength(1)]
        public string OpenQualifyingFlag { get; set; }

        public byte? NumberOfWildCardsMainDraw { get; set; }

        public byte? NumberOfWildCardsQualifying { get; set; }

        public byte? NumberOfSpecialExempts { get; set; }

        public byte? NumberOfMainDrawWithdrawals { get; set; }

        public byte? NumberOfQualifyingWithdrawals { get; set; }

        [StringLength(1)]
        public string PublishedEntryListFlag { get; set; }

        [StringLength(20)]
        public string LockedBy { get; set; }

        public byte? LastSortedBy { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(20)]
        public string LastUpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] TStamp { get; set; }

        [StringLength(1)]
        public string EntryDeadlineBlackOutLapsedFlag { get; set; }

        [StringLength(1)]
        public string EntryEmbargoInforceFlag { get; set; }

        public DateTime? SeedingRankDate { get; set; }

        [StringLength(100)]
        public string SignInDeadline { get; set; }

        public virtual Event Event { get; set; }
    }
}
