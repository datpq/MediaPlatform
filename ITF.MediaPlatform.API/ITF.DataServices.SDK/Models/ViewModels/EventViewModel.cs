using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class EventViewModel
    {
        public string Name { get; set; }
        public string EventDesc { get; set; }
        public string EventCode { get; set; }
        public string DrawsheetType { get; set; }
        public string DivisionCode { get; set; }
        public string ZoneCode { get; set; }
        public ICollection<RoundViewModel> Rounds { get; set; }
    }

    public class EventResultsByYearViewModel
    {
        public string Name { get; set; }
        public string EventDesc { get; set; }
        public string EventCode { get; set; }
        public string DrawsheetType { get; set; }
        public string DivisionCode { get; set; }
        public string ZoneCode { get; set; }
        public ICollection<RoundResultsByYearViewModel> Rounds { get; set; }
    }
}
