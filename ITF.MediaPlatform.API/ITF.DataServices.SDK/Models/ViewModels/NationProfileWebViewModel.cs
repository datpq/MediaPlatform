using System;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class NationProfileWebViewModel
    {
        public string OfficialName { get; set; }
        public string AdminName { get; set; }
        public string WebsiteURL { get; set; }
        public string Captain { get; set; }
        public string CaptainId { get; set; }
        public string NextTieDate { get; set; }
        public string NextTieDesc { get; set; }
        public string NextTieId { get; set; }
        public string NextTiePlayStatusCode { get; set; }
        public string LastTieDate { get; set; }
        public string LastTieDesc { get; set; }
        public string LastTieId { get; set; }
        public string History { get; set; }
        public string Record { get; set; }
    }

    public class NationProfileWebViewModelOld : NationProfileWebViewModel
    {
        [Obsolete]
        public string NextTieDateEN { get; set; }
        [Obsolete]
        public string NextTieDateES { get; set; }
        [Obsolete]
        public string NextTieDescEN { get; set; }
        [Obsolete]
        public string NextTieDescES { get; set; }
        [Obsolete]
        public string NextTieIdEN { get; set; }
        [Obsolete]
        public string NextTieIdES { get; set; }

        [Obsolete]
        public string LastTieDateEN { get; set; }
        [Obsolete]
        public string LastTieDateES { get; set; }
        [Obsolete]
        public string LastTieDescEN { get; set; }
        [Obsolete]
        public string LastTieDescES { get; set; }
        [Obsolete]
        public string LastTieIdEN { get; set; }
        [Obsolete]
        public string LastTieIdES { get; set; }

        [Obsolete]
        public string HistoryEN { get; set; }
        [Obsolete]
        public string HistoryES { get; set; }
        [Obsolete]
        public string RecordEN { get; set; }
        [Obsolete]
        public string RecordES { get; set; }
    }
}
