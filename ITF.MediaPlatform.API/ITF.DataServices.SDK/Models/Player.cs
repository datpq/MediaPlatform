namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Player")]
    public partial class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerID { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string FamilyName { get; set; }

        [StringLength(40)]
        [Column(TypeName = "VARCHAR")]
        public string GivenName { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string NationalityCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string NationalityDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public short? BirthYear { get; set; }

        public byte? TennisCategoryActivePlayingList { get; set; }

        public byte? TennisCategoryPlayedList { get; set; }

        public int? DataExchangePlayerId { get; set; }
    }
}
