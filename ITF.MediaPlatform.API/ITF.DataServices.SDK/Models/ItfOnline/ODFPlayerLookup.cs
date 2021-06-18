namespace ITF.DataServices.SDK.Models.ItfOnline
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ODFPlayerLookup")]
    public partial class ODFPlayerLookup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OdfPlayerId { get; set; }

        public int DataExchangePlayerId { get; set; }
    }
}
