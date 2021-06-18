using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models.Cup
{
    [Table("PlayerTeamCompetitionParticipationSummary")]
    public class PlayerTeamCompetitionParticipationSummary
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        public int? DataExchangePlayerId { get; set; }

        [StringLength(40)]
        public string FamilyName { get; set; }

        [StringLength(40)]
        public string GivenName { get; set; }

        [StringLength(3)]
        public string NationalityCode { get; set; }

        [StringLength(100)]
        public string Nationality { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string TennisCategoryCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string TourCategoryCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FirstAppearanceDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? LastAppearanceDate { get; set; }

        public int? HomeAndAwayAppearances { get; set; }

        public int? RoundRobinEventAppearances { get; set; }

        public int? TotalAppearances { get; set; }

        public int? TotalAppearanceYears { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FirstNominationsDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? LastNominationsDate { get; set; }

        public int? HomeAndAwayNominations { get; set; }

        public int? RoundRobinEventNominations { get; set; }

        public int? TotalNominations { get; set; }

        public int? TotalNominationYears { get; set; }

        public bool? IsEligibleForCommitmentAward { get; set; }

        public bool? IsPlayerActive { get; set; }

        [StringLength(3)]
        public string RepresentingNationCode { get; set; }

        [StringLength(100)]
        public string RepresentingNationDesc { get; set; }
    }
}