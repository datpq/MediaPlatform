namespace ITF.DataServices.SDK.Models.Cms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ITFNationBaseline")]
    public partial class ITFNationBaseline
    {
        [Required]
        [StringLength(3)]
        public string NationCode { get; set; }

        [StringLength(100)]
        public string NationName { get; set; }

        [StringLength(1)]
        public string CurrentNationFlag { get; set; }

        [StringLength(100)]
        public string NationNameEnglish { get; set; }

        [StringLength(100)]
        public string NationNameFrench { get; set; }

        [StringLength(100)]
        public string NationNameSpanish { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NationId { get; set; }

        [StringLength(100)]
        public string NationNameShort { get; set; }

        [StringLength(100)]
        public string NationNameSpanishShort { get; set; }
    }
}
