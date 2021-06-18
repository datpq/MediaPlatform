using System;
using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class NationWinLossRecordsViewModel
    {
        public int Year { get; set; }
        public List<NationActivityTieViewModel> Ties { get; set; }
    }

    public class NationActivityTieViewModel
    {
        public string TieId { get; set; }
        public string DivisionCode { get; set; }
        public string ZoneCode { get; set; }
        public string GroupCode { get; set; }
        public string ClassificationCode { get; set; }
        public string Round { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string OpponentNationCode { get; set; }
        public string OpponentNationName { get; set; }
        public string HostNationCode { get; set; }
        public string SurfaceCode { get; set; }
        public string IndoorOutdoor { get; set; }
        public string Score { get; set; }
        public string ResultCode { get; set; }

        [Obsolete]
        public string StartDateEN { get; set; }
        [Obsolete]
        public string StartDateES { get; set; }
        [Obsolete]
        public string EndDateEN { get; set; }
        [Obsolete]
        public string EndDateES { get; set; }
    }
}
