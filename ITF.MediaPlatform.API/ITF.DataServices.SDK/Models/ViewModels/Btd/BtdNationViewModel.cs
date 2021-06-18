using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Btd
{
    public class BtdNationViewModel
    {
        public NationProfileWebViewModel NationProfile { get; set; }
        public NationViewModel Nation { get; set; }
        public NationAllTimeRecordsViewModel NationAllTimeRecords { get; set; }
        public TieViewModel NextTie { get; set; }
        public ICollection<PlayerViewModelCore> RecentPlayers { get; set; }
        public ICollection<BtdRecentResultViewModel> RecentResults { get; set; }
    }

    public class BtdMyTeamViewModel
    {
        public string NextTieDate { get; set; }
        public string NextTieDesc { get; set; }
        public string NextTieId { get; set; }
        public string LastTieDate { get; set; }
        public string LastTieDesc { get; set; }
        public string LastTieId { get; set; }
        public string History { get; set; }
        public TieViewModel NextTie { get; set; }
        public TieViewModel LastTie { get; set; }
    }

    public class BtdRecentResultViewModel
    {
        public int Year { get; set; }
        public ICollection<BtdResultByYearViewModel> Results { get; set; }
        public ICollection<NationPlayersWinLossRecords> Players;
    }

    public class BtdResultByYearViewModel
    {
        public string TieId { get; set; }
        public int TieYear { get; set; }
        public string EventDivision { get; set; }
        public string EventRound { get; set; }
        public string Side1NationCode { get; set; }
        public string Side1NationName { get; set; }
        public string Side1Score { get; set; }
        public string Side2NationCode { get; set; }
        public string Side2NationName { get; set; }
        public string Side2Score { get; set; }

        public string ResultCode { get; set; }
        public string Score { get; set; }
    }

    public class BtdNationPlayersViewModel
    {
        public ICollection<PlayerViewModelCoreCore> AllPlayers { get; set; }
        public ICollection<PlayerViewModelCore> RecentPlayers { get; set; }
    }
}
