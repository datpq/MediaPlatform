using System;
using System.Collections.Generic;
using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    [TsClass]
    public class EventViewModel
    {
        public DateTime EndDate { get; set; }
        public string EventClassificationCode { get; set; }
        public string EventClassificationDesc { get; set; }
        public int EventId { get; set; }
        public string HostNationCode { get; set; }
        public string HostNationName { get; set; }
        public string IndoorOutdoorFlag { get; set; }
        public string MatchTypeCode { get; set; }
        public string MatchTypeDesc { get; set; }
        public string Name { get; set; }
        public ICollection<PlayerActivityMatchViewModel> PlayerActivityMatches { get; set; }
        public string PlayerTypeCode { get; set; }
        public string PlayerTypeDesc { get; set; }
        public DateTime StartDate { get; set; }
        public string SurfaceDesc { get; set; }
        public string Venue { get; set; }
        public int Year { get; set; }
    }
}
