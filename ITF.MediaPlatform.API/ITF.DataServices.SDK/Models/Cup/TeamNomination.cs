namespace ITF.DataServices.SDK.Models.Cup
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TeamNomination")]
    public partial class TeamNomination
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TeamNominationId { get; set; }

        public int TournamentId { get; set; }

        public int TeamCompDivisionalGroupId { get; set; }

        [Required]
        [StringLength(5)]
        public string TourCategoryCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CalendarYear { get; set; }

        [Required]
        [StringLength(50)]
        public string Division { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(3)]
        public string DivisionCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Zone { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(3)]
        public string ZoneCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Nation { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(3)]
        public string NationCode { get; set; }

        public byte? Seeding { get; set; }

        public int? CaptainPlayerId { get; set; }

        [StringLength(100)]
        public string CaptainName { get; set; }

        [StringLength(40)]
        public string CaptainFamilyName { get; set; }

        [StringLength(40)]
        public string CaptainGivenName { get; set; }

        [StringLength(100)]
        public string CaptainNationality { get; set; }

        [StringLength(3)]
        public string CaptainNationalityCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CaptainBirthDate { get; set; }

        [StringLength(200)]
        public string PublicTournamentDetailId { get; set; }

        [StringLength(100)]
        public string PublicTournamentId { get; set; }

        public int? CaptainDataExchangePlayerId { get; set; }
    }
}
