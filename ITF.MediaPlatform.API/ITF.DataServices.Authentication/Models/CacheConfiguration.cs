namespace ITF.DataServices.Authentication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CacheConfiguration")]
    public partial class CacheConfiguration
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Application { get; set; }

        [StringLength(250)]
        public string KeyName { get; set; }

        [Required]
        [StringLength(250)]
        public string KeyPattern { get; set; }

        public int Timeout { get; set; }

        public bool Enabled { get; set; }
    }
}
