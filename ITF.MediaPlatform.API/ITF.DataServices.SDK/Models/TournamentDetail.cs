namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TournamentDetail")]
    public partial class TournamentDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TournamentDetailID { get; set; }

        public int TournamentID { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentDetailID { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string PublicTournamentId { get; set; }

        [StringLength(150)]
        [Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public short? Year { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string DivisionCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string DivisionDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string ZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ZoneDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string SubZoneCode { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string SubZoneDesc { get; set; }

        [StringLength(3)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string HostNationName { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Location { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string SiteName { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceDesc { get; set; }

        public int? SurfaceTypeID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string SurfaceTypeDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string IndoorOutdoorFlag { get; set; }

        public int? BallID { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string BallDesc { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string BallSpeedCode { get; set; }

        [StringLength(25)]
        [Column(TypeName = "VARCHAR")]
        public string BallSpeedDesc { get; set; }
    }
}
