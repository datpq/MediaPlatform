using System;
using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class CupHeartAwardData
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Votes { get; set; }

        public DateTime LastUpdated { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
        public string NationCode { get; set; }
    }
}
