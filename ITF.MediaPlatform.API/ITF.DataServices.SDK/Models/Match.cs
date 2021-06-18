namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Match")]
    public partial class Match
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MatchID { get; set; }

        public int TournamentID { get; set; }

        public int EventID { get; set; }

        public int? TieID { get; set; }

        public int? Side1Player1ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side1Player1FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side1Player1GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side1Player1NationalityCode { get; set; }

        public int? Side1Player2ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side1Player2FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side1Player2GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side1Player2NationalityCode { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side1TieNationCode { get; set; }

        public short? Side1Seeding { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string Side1EntryClassificationCode { get; set; }

        public int? Side2Player1ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side2Player1FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side2Player1GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side2Player1NationalityCode { get; set; }

        public int? Side2Player2ID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side2Player2FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string Side2Player2GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side2Player2NationalityCode { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string Side2TieNationCode { get; set; }

        public short? Side2Seeding { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string Side2EntryClassificationCode { get; set; }

        public byte? DrawsheetRoundNumber { get; set; }

        [StringLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string DrawsheetRoundCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string DrawsheetRoundDesc { get; set; }

        public byte? DrawsheetPositionMatch { get; set; }

        public short? DrawsheetPositionSide1 { get; set; }

        public short? DrawsheetPositionSide2 { get; set; }

        public byte? WinningSide { get; set; }

        public byte? ScoreSet1Side1 { get; set; }

        public byte? ScoreSet1Side2 { get; set; }

        public byte? ScoreSet1LosingTieBreak { get; set; }

        public byte? ScoreSet2Side1 { get; set; }

        public byte? ScoreSet2Side2 { get; set; }

        public byte? ScoreSet2LosingTieBreak { get; set; }

        public byte? ScoreSet3Side1 { get; set; }

        public byte? ScoreSet3Side2 { get; set; }

        public byte? ScoreSet3LosingTieBreak { get; set; }

        public byte? ScoreSet4Side1 { get; set; }

        public byte? ScoreSet4Side2 { get; set; }

        public byte? ScoreSet4LosingTieBreak { get; set; }

        public byte? ScoreSet5Side1 { get; set; }

        public byte? ScoreSet5Side2 { get; set; }

        public byte? ScoreSet5LosingTieBreak { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string Score { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ScoreReversed { get; set; }

        [StringLength(2)]
        [Column(TypeName = "VARCHAR")]
        public string PlayStatusCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string PlayStatusDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string ResultStatusCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ResultStatusDesc { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentId { get; set; }

        [StringLength(200)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTieId { get; set; }

        public int? Side1Player1DataExchangePlayerId { get; set; }

        public int? Side1Player2DataExchangePlayerId { get; set; }

        public int? Side2Player1DataExchangePlayerId { get; set; }

        public int? Side2Player2DataExchangePlayerId { get; set; }

        public bool? IsMatchScoreDiscrepancy { get; set; }
    }
}
