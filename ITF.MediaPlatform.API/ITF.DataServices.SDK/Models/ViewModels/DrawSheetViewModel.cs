using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class DrawSheetViewModel
    {
        public int Year { get; set; }
        public ICollection<EventViewModel> Events { get; set; }
    }

    public class ResultsByYearViewModel
    {
        public int Year { get; set; }
        public ICollection<EventResultsByYearViewModel> Events { get; set; }
    }

    public class EventYearViewModel
    {
        public int Year { get; set; }
    }
}
