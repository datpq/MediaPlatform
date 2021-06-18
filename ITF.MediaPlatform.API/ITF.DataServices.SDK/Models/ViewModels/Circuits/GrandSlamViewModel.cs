using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    public class GrandSlamViewModel
    {
        public int Year { get; set; }
        public string HostNationCode { get; set; }
        public string Name { get; set; }
        public string MediaName { get; set; }
        public string Venue { get; set; }
        public string Dates { get; set; }
        public ICollection<GrandSlamEventViewModel> Events { get; set; } //Event
    }

    public class GrandSlamEventViewModel
    {
        public string PlayerType { get; set; }
        public string MatchType { get; set; }
        public string Surface { get; set; }
        public string IndoorOutdoor { get; set; }
        public string FinalSide1Player1GivenName { get; set; }
        public string FinalSide1Player1FamilyName { get; set; }
        public string FinalSide1Player1NationCode { get; set; }
        public string FinalSide1Player2GivenName { get; set; }
        public string FinalSide1Player2FamilyName { get; set; }
        public string FinalSide1Player2NationCode { get; set; }
        public string FinalSide2Player1GivenName { get; set; }
        public string FinalSide2Player1FamilyName { get; set; }
        public string FinalSide2Player1NationCode { get; set; }
        public string FinalSide2Player2GivenName { get; set; }
        public string FinalSide2Player2FamilyName { get; set; }
        public string FinalSide2Player2NationCode { get; set; }
        public int FinalWinningSide { get; set; }
        public string FinalScore { get; set; }
        public ICollection<GrandSlamPlayerViewModel> Players { get; set; }
    }

    public class GrandSlamPlayerViewModel
    {
        public string PlayerGivenName { get; set; }
        public string PlayerFamilyName { get; set; }
        public string PlayerNationCode { get; set; }
        public int PlayerId { get; set; }
        public int? Seeding { get; set; }
        public string FinalPositionCode { get; set; }
    }
}
