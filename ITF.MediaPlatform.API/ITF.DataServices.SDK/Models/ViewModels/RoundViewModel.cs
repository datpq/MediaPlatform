using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class RoundViewModel
    {
        public string RoundDesc { get; set; }
        public int RoundNumber { get; set; }
        public string RoundStartDate { get; set; }
        public string RoundEndDate { get; set; }
        public ICollection<TieDrawViewModel> Ties { get; set; }
    }

    public class RoundResultsByYearViewModel
    {
        public string RoundDesc { get; set; }
        public int RoundNumber { get; set; }
        public ICollection<TieDrawFullViewModel> Ties { get; set; }
    }
}
