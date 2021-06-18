using System.Collections.Generic;
using ITF.DataServices.SDK.Models.ViewModels;

namespace ITF.MediaPlatform.API.ViewModels
{
    public class BtdTournamentViewModel : TournamentViewModel
    {
        public string SurfaceCode2 { get; set; }
    }

    public class BtdRoundRobinEventAppViewModel
    {
        public ICollection<BtdTournamentViewModel> Tournaments { get; set; }
        public ICollection<RoundRobinEventViewModel> Events { get; set; }
    }
}