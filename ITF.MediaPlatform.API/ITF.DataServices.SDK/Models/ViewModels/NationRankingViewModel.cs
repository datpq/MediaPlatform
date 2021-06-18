using System;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class NationRankingViewModel : NationRankingCoreViewModel
    {
        public string Movement { get; set; }
        public string Played { get; set; }
        public string RankDate { get; set; }
    }

    public class NationRankingCoreViewModel
    {
        public string NationCode { get; set; }
        public string Nation { get; set; }
        public int Rank { get; set; }
        public string RankEqual { get; set; }
        public decimal? Points { get; set; }

        [Obsolete("Please use Nation instead", true)]
        public string NationEN { get; set; }
        [Obsolete("Please use Nation instead", true)]
        public string NationES { get; set; }
    }
}
