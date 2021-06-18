using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class TieDetailsViewModel
    {
        public string TieId { get; set; }
        public string PublicTieId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Venue { get; set; }
        public string TimeOfPlayDay1 { get; set; }
        public string TimeOfPlayDay2 { get; set; }
        public string TimeOfPlayDay3 { get; set; }
        public string HostNationCode { get; set; }
        public string HostNationName { get; set; }
        public string Side1NationCode { get; set; }
        public string Side1NationName { get; set; }
        public string Side2NationCode { get; set; }
        public string Side2NationName { get; set; }
        public string Score { get; set; }
        public string PlayStatusCode { get; set; }
        public string SurfaceCode { get; set; }
        public string Surface { get; set; }
        public string SurfaceTypeDesc { get; set; }
        public string BallDesc { get; set; }
        public string IndoorOutdoor { get; set; }
    }

    public class TieDetailsWebViewModel : TieDetailsViewModel
    {
        public string EventName { get; set; }
        public string RoundDesc { get; set; }
        public int Side1NationH2HWinCount { get; set; }
        public int Side2NationH2HWinCount { get; set; }
        public string Side1Score { get; set; }
        public string Side2Score { get; set; }
        public bool IsSquadNominated { get; set; }
        public bool IsDrawsMade { get; set; }
        public string ResultStatusCode { get; set; }
    }

    public class TieDetailsAppViewModel : TieDetailsWebViewModel
    {
        public string Report { get; set; }
        public ICollection<TieResultsWebViewModel> Results { get; set; }
        public ICollection<TieNominationViewModel> Nominations { get; set; }
    }
}
