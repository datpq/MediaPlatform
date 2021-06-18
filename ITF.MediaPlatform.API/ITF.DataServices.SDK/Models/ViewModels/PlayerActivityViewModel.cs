using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class PlayerActivityViewModelBase
    {
        public int TotalWinTotal { get; set; }
        public int TotalWinSingles { get; set; }
        public int TotalWinDoubles { get; set; }
        public int TotalLossTotal { get; set; }
        public int TotalLossSingles { get; set; }
        public int TotalLossDoubles { get; set; }
        public int ClayWinTotal { get; set; }
        public int ClayWinSingles { get; set; }
        public int ClayWinDoubles { get; set; }
        public int ClayLossTotal { get; set; }
        public int ClayLossSingles { get; set; }
        public int ClayLossDoubles { get; set; }
        public int HardWinTotal { get; set; }
        public int HardWinSingles { get; set; }
        public int HardWinDoubles { get; set; }
        public int HardLossTotal { get; set; }
        public int HardLossSingles { get; set; }
        public int HardLossDoubles { get; set; }
        public int GrassWinTotal { get; set; }
        public int GrassWinSingles { get; set; }
        public int GrassWinDoubles { get; set; }
        public int GrassLossTotal { get; set; }
        public int GrassLossSingles { get; set; }
        public int GrassLossDoubles { get; set; }
        public int CarpetWinTotal { get; set; }
        public int CarpetWinSingles { get; set; }
        public int CarpetWinDoubles { get; set; }
        public int CarpetLossTotal { get; set; }
        public int CarpetLossSingles { get; set; }
        public int CarpetLossDoubles { get; set; }
        public int UnknownWinTotal { get; set; }
        public int UnknownWinSingles { get; set; }
        public int UnknownWinDoubles { get; set; }
        public int UnknownLossTotal { get; set; }
        public int UnknownLossSingles { get; set; }
        public int UnknownLossDoubles { get; set; }
        public int IndoorWinTotal { get; set; }
        public int IndoorWinSingles { get; set; }
        public int IndoorWinDoubles { get; set; }
        public int IndoorLossTotal { get; set; }
        public int IndoorLossSingles { get; set; }
        public int IndoorLossDoubles { get; set; }
        public int OutdoorWinTotal { get; set; }
        public int OutdoorWinSingles { get; set; }
        public int OutdoorWinDoubles { get; set; }
        public int OutdoorLossTotal { get; set; }
        public int OutdoorLossSingles { get; set; }
        public int OutdoorLossDoubles { get; set; }
    }

    public class PlayerActivityViewModel : PlayerActivityViewModelBase
    {
        public List<TieViewModel> Ties { get; set; }
    }

    public class PlayerActivityViewModelOld : PlayerActivityViewModelBase
    {
        public List<TieViewModelOld> Ties { get; set; }
    }
}
