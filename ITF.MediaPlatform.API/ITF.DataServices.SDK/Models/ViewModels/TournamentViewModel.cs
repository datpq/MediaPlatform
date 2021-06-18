using System;
using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class TournamentViewModel
    {
        public string TournamentId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Year { get; set; }
        public string DivisionCode { get; set; }
        public string ZoneCode { get; set; }
        public string SubZoneCode { get; set; }
        public string Venue { get; set; }
        public string Location { get; set; }
        public string HostNationCode { get; set; }
        public string HostNationName { get; set; }
        public string SurfaceCode { get; set; }
        public string SurfaceDesc { get; set; }
    }

    public class RoundRobinEventAppViewModel
    {
        public ICollection<TournamentViewModel> Tournaments { get; set; }
        public ICollection<RoundRobinEventViewModel> Events { get; set; }
    }
}
