namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NationSquad")]
    public partial class NationSquad
    {
        public int SquadID { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TieID { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string NationCode { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string NationName { get; set; }

        public int? CaptainPlayerID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string CaptainFamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string CaptainGivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string CaptainNationalityCode { get; set; }

        [Column(TypeName = "text")]
        public string CaptainNote { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string CaptainPlayingFlag { get; set; }

        public int? Player1ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player1FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player1GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Player1NationalityCode { get; set; }

        public int? Player2ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player2FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player2GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Player2NationalityCode { get; set; }

        public int? Player3ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player3FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player3GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Player3NationalityCode { get; set; }

        public int? Player4ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player4FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player4GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Player4NationalityCode { get; set; }

        public int? Player5ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player5FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Player5GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Player5NationalityCode { get; set; }

        [Required]
        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string WebMastersChoiceFlag { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentId { get; set; }

        [StringLength(200)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTieId { get; set; }

        public int? CaptainDataExchangePlayerId { get; set; }

        public int? Player1DataExchangePlayerId { get; set; }

        public int? Player2DataExchangePlayerId { get; set; }

        public int? Player3DataExchangePlayerId { get; set; }

        public int? Player4DataExchangePlayerId { get; set; }

        public int? Player5DataExchangePlayerId { get; set; }
    }
}
