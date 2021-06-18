using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    public class HeadToHeadViewModel
    {
        public ICollection<HeadToHeadMatchActivityViewModel> Matches { get; set; }
        public BasePlayerProfileViewModel OpponentPlayer { get; set; }
        public BasePlayerProfileViewModel Player { get; set; }
    }
}
