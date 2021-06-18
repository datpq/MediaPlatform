using System;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class NationViewModel : NationCoreViewModel
    {
        public int Champion { get; set; }
        public string ChampionYears { get; set; }
        public string Ranking { get; set; }
        public int? FirstYearPlayed { get; set; }
        public int YearsPlayed { get; set; }
        public string TiesPlayed { get; set; }
        public string YearsInWG { get; set; }
        public string LastTiePlayed { get; set; }
    }

    public class NationViewModelOld : NationCoreViewModelOld
    {
        public int Champion { get; set; }
        public string ChampionYears { get; set; }
        public string Ranking { get; set; }
        public int? FirstYearPlayed { get; set; }
        public int YearsPlayed { get; set; }
        public string TiesPlayed { get; set; }
        public string YearsInWG { get; set; }
        public string LastTiePlayed { get; set; }
    }

    public class NationCoreViewModelOld : NationCoreViewModel
    {
        [Obsolete("Please use Nation instead", true)]
        public string NationEN { get; set; }
        [Obsolete("Please use NationI instead", true)]
        public string NationENI { get; set; }
        [Obsolete("Please use Nation instead", true)]
        public string NationES { get; set; }
        [Obsolete("Please use NationI instead", true)]
        public string NationESI { get; set; }
    }

    public class NationCoreViewModel
    {
        public string NationCode { get; set; }
        public string Nation { get; set; }
        public string NationI { get; set; }
    }
}
