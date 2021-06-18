using System.Collections.Generic;
using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    [TsClass]
    public class OlympicsPlayersViewModel : OlympicsViewModel
    {
        public List<PlayersViewModel> Players { get; set; }
    }
}
