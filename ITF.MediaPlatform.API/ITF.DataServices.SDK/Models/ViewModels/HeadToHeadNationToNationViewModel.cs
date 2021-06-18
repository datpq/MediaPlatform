using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class HeadToHeadNationToNationViewModel
    {
        public string NationName { get; set; }
        public string NationNameES { get; set; }
        public string NationCode { get; set; }
        public string OppositionNationName { get; set; }
        public string OppositionNationNameES { get; set; }
        public string OppositionNationCode { get; set; }
        public int WinTotal { get; set; }
        public int LossTotal { get; set; }
        public int WinClay { get; set; }
        public int LossClay { get; set; }
        public int WinHard { get; set; }
        public int LossHard { get; set; }
        public int WinGrass { get; set; }
        public int LossGrass { get; set; }
        public int WinCarpet { get; set; }
        public int LossCarpet { get; set; }
        public int WinUnknown { get; set; }
        public int LossUnknown { get; set; }
        public int WinIndoor { get; set; }
        public int LossIndoor { get; set; }
        public int WinOutdoor { get; set; }
        public int LossOutdoor { get; set; }
        public ICollection<HeadToHeadNationToNationTie> Ties { get; set; }
    }

    public class HeadToHeadNationToNationTie
    {
        public string PublicTieId { get; set; }
        public string ResultDesc { get; set; }
        public string Result { get; set; }
        public string Venue { get; set; }
        public int? Year { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SurfaceCode { get; set; }
        public string IndoorOutdoorCode { get; set; }
        public string Group { get; set; }
        public string Zone { get; set; }
        public string DrawType { get; set; }
        public string DrawClass { get; set; }
        public string Round { get; set; }
        public string SubGroupCode { get; set; }
        public string PlayStatusCode { get; set; }
    }
}
