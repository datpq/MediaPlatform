namespace ITF.DataServices.SDK.Models.Cup
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TeamNominationPlayer")]
    public partial class TeamNominationPlayer
    {
        public int TournamentId { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TeamNominationId { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte TeamNominationPosition { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        [StringLength(40)]
        public string FamilyName { get; set; }

        [StringLength(40)]
        public string GivenName { get; set; }

        [StringLength(100)]
        public string Nationality { get; set; }

        [StringLength(3)]
        public string NationalityCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        public int? TeamCompDivisionalGroupId { get; set; }

        [StringLength(100)]
        public string PublicTournamentId { get; set; }

        [StringLength(200)]
        public string PublicTournamentDetailId { get; set; }

        public int? DataExchangePlayerId { get; set; }
    }
}
