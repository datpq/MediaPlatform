namespace ITF.DataServices.SDK.Models.Baseline02
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MatchPlayer")]
    public partial class MatchPlayer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MatchPlayerID { get; set; }

        public int PlayerID { get; set; }

        [StringLength(60)]
        public string FullName { get; set; }

        [StringLength(40)]
        public string FamilyName { get; set; }

        [StringLength(40)]
        public string GivenName { get; set; }

        [StringLength(60)]
        public string NickName { get; set; }

        [StringLength(40)]
        public string MiddleName { get; set; }

        [StringLength(3)]
        public string NationalityCode { get; set; }

        [Required]
        [StringLength(1)]
        public string CurrentMatchPlayerFlag { get; set; }

        public DateTime LastUpdated { get; set; }

        [Required]
        [StringLength(20)]
        public string LastUpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] TStamp { get; set; }
    }
}
