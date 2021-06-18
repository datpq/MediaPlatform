namespace ITF.DataServices.SDK.Models.WorldNet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Baseline360Export
    {
        public int? BID { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string OrgCode { get; set; }

        [StringLength(160)]
        public string Organisation { get; set; }

        [StringLength(100)]
        public string OrgEmail { get; set; }

        [StringLength(200)]
        public string OrgWWW { get; set; }

        [StringLength(10)]
        public string Class { get; set; }

        [StringLength(100)]
        public string OrgNationName { get; set; }

        [StringLength(200)]
        public string AddressLine1 { get; set; }

        [StringLength(200)]
        public string AddressLine2 { get; set; }

        [StringLength(200)]
        public string AddressLine3 { get; set; }

        [StringLength(200)]
        public string AddressLine4 { get; set; }

        [StringLength(200)]
        public string AddressLine5 { get; set; }

        [StringLength(50)]
        public string PostCode { get; set; }

        [StringLength(50)]
        public string Tel { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        [StringLength(10)]
        public string Title { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string LastName { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string RoleCode { get; set; }

        [StringLength(10)]
        public string RAAffiliation { get; set; }

        [StringLength(100)]
        public string OrgNationCode { get; set; }

        [StringLength(100)]
        public string AddressNationCode { get; set; }

        [StringLength(100)]
        public string AddressNationName { get; set; }
    }
}
