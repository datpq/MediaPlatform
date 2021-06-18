using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models
{
    public partial class PlayerExternal
    {
        [Key]
        public int PlayerID { get; set; }

        public int PlayerExternalID { get; set; }
    }
}
