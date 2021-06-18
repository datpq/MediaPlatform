using System;
using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    public class BasePlayerProfileViewModel
    {
        public int AgeBeganTennis { get; set; }
        public int AgeTurnPro { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public int BirthYear { get; set; }
        public string FamilyName { get; set; }
        public string Gender { get; set; }
        public string GivenName { get; set; }
        public int? HeadshotAssetId { get; set; }
        public string Height { get; set; }
        public string NationalityCode { get; set; }
        public string NationalityDesc { get; set; }
        public int PlayerID { get; set; }
        public string Residence { get; set; }
        public string TennisHands { get; set; }
        public string Weight { get; set; }
    }

    [TsClass]
    public class PlayerProfileViewModel : BasePlayerProfileViewModel
    {
        public string RankCareerHighDoublesRollover { get; set; }
        public DateTime RankCareerHighDoublesRolloverDate { get; set; }
        public string RankCareerHighSinglesRollover { get; set; }
        public DateTime RankCareerHighSinglesRolloverDate { get; set; }
        public string RankCurrentDoublesRollover { get; set; }
        public string RankCurrentSinglesRollover { get; set; }
    }

    [TsClass]
    public class WheelchairPlayerProfileViewModel : BasePlayerProfileViewModel
    {
        public string RankCurrentDoubles { get; set; }
        public string RankCurrentDoublesQuad { get; set; }
        public string RankCurrentSingles { get; set; }
        public string RankCurrentSinglesQuad { get; set; }
    }
}
