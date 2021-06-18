using System;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class PlayerViewModelCoreCore
    {
        public int PlayerId { get; set; }
        public string NationCode { get; set; }
        public string NationName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
    }

    public class PlayerViewModelCore : PlayerViewModelCoreCore
    {
        public int? HeadshotImgId { get; set; }
        public string HeadshotUrl { get; set; }
    }

    public class PlayerViewModelCoreOld : PlayerViewModelCore
    {
        [Obsolete]
        public string NationNameEN { get; set; }
        [Obsolete]
        public string NationNameES { get; set; }
        [Obsolete]
        public string BirthDateEN { get; set; }
        [Obsolete]
        public string BirthDateES { get; set; }
    }

    public class PlayerViewModel : PlayerViewModelCoreOld
    {
        public string BirthPlace { get; set; }
        public string Residence { get; set; }
        public string Plays { get; set; }
        public string RankSingles { get; set; }
        public string RankDoubles { get; set; }
        public string FirstYearPlayed { get; set; }
        public string TotalNominations { get; set; }
        public int TiesPlayed { get; set; }
        public string WLSingles { get; set; }
        public string WLDoubles { get; set; }
        public string WLTotal { get; set; }
        public bool CommitmentAward { get; set; }
    }
}
