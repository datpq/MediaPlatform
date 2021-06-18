using System;
using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class TieViewModelOld : TieViewModel
    {
        [Obsolete]
        public string TieDateEN { get; set; }
        [Obsolete]
        public string TieDateES { get; set; }
    }

    public class TieViewModel
    {
        public string TieId { get; set; }
        public string TieResult { get; set; }
        public int TieYear { get; set; }
        public string TieDate { get; set; }
        public string TieVenue { get; set; }
        public string EventDivision { get; set; }
        public string EventZone { get; set; }
        public string EventSubGroup { get; set; }
        public string EventClass { get; set; }
        public string EventDrawStructure { get; set; }
        public string EventRound { get; set; }
        public string SurfaceCode { get; set; }
        public string IndoorOutdoor { get; set; }
        public string Side1NationCode { get; set; }
        public string Side1NationName { get; set; }
        public string Side1Score { get; set; }
        public string Side2NationCode { get; set; }
        public string Side2NationName { get; set; }
        public string Side2Score { get; set; }
        public object HostNationCode { get; set; }
        public string PlayStatusCode { get; set; }
        public List<MatchViewModel> Matches { get; set; }
    }
}
