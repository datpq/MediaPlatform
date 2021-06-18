using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models
{
    public partial class NationTranslated
    {
        [Key]
        [Column(TypeName = "VARCHAR")]
        public string NationCode { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string NationName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string NationNameSpanish { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string NationNameFrench { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string CurrentNationFlag { get; set; }
        public int? NationId { get; set; }
    }
}
