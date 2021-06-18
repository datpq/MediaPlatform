using System;
using System.Collections.Generic;
using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    [TsClass]
    public class DrawsheetViewModel
    {
        public DateTime EndDate { get; set; }
        public string EventClassificationDesc { get; set; }
        public int EventID { get; set; }
        public string HostNationCode { get; set; }
        public string HostNationName { get; set; }
        public string IndoorOutdoorFlag { get; set; }
        public ICollection<MatchViewModel> Matches { get; set; }
        public string MatchTypeDesc { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public string SurfaceDesc { get; set; }
        public DrawsheetViewModel ThirdFourthEvent { get; set; }
        public string Venue { get; set; }
        public int Year { get; set; }
    }
}
