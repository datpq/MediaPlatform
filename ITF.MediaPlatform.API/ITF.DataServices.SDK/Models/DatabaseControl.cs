using System;
using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models
{
    public partial class DatabaseControl
    {
        public string DB1 { get; set; }
        public string DB2 { get; set; }

        [Key]
        public string DBLive { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
