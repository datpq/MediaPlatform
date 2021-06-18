namespace ITF.DataServices.SDK.Models.Baseline02
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MatchPlayerEventInfo")]
    public partial class MatchPlayerEventInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MatchPlayerEventInfoID { get; set; }

        public int MatchPlayerID { get; set; }

        public int EventID { get; set; }

        [StringLength(5)]
        public string EntryClassificationCode { get; set; }

        [StringLength(4)]
        public string FinalPositionCode { get; set; }

        [Column(TypeName = "money")]
        public decimal? PrizeMoney { get; set; }

        public short? Seeding { get; set; }

        public decimal? RoundPoints { get; set; }

        public decimal? QualifyPoints { get; set; }

        public decimal? BonusPoints { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(20)]
        public string LastUpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] TStamp { get; set; }

        public byte? RoundRobinPosition { get; set; }
    }
}
