using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models.ItfOnline
{
    public abstract class BaseOlympics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Year { get; set; }

        [Required]
        [StringLength(300)]
        public string TournamentName { get; set; }

        [Required]
        [StringLength(3)]
        public string HostNationCode { get; set; }

        [Required]
        [StringLength(100)]
        public string HostNationName { get; set; }

        [StringLength(50)]
        public string HostCity { get; set; }
    }
}
