using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class RoundRobinNominationViewModel
    {
        public int TeamNominationId { get; set; }
        public string Nation { get; set; }
        public string NationCode { get; set; }
        public int? Seeding { get; set; }
        public string CaptainFamilyName { get; set; }
        public string CaptainGivenName { get; set; }
        public int? CaptainPlayerId { get; set; }
        public string CaptainNationCode { get; set; }
        public ICollection<PlayerViewModelCoreOld> Players { get; set; }
    }
}
