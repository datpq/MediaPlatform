using System;
using System.Collections.Generic;
using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    [TsClass]
    public class TournamentViewModel
    {
        public DateTime EndDate { get; set; }
        public ICollection<EventViewModel> Events { get; set; }
        public string HostNationName { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public string TourCategoryDesc { get; set; }
        public string Venue { get; set; }
        public int Year { get; set; }
    }
}
