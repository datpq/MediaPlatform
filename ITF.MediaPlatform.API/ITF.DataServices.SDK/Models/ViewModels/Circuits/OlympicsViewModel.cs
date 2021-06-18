using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    public class OlympicsViewModel
    {
        public string HostCity { get; set; }
        public string HostNationCode { get; set; }
        public string HostNationName { get; set; }
        public string TournamentName { get; set; }
        public short Year { get; set; }
    }

    public class OlympicTennisViewModel
    {
        public int Year { get; set; } //Olympic Year
        public string HostCity { get; set; } //Olympic Host City
        public string HostNation { get; set; } //Olympic Host Nation
        public string HostNationCode { get; set; } //Olympic Host Nation Code
        public ICollection<OlympicTennisEventViewModel> Events { get; set; } //Event
    }

    public class OlympicTennisEventViewModel
    {
        public string PlayerType { get; set; }
        public string MatchType { get; set; }
        public string EventType { get; set; }
        public string Venue { get; set; } //Tennis Venue
        public string Surface { get; set; } //Court Surface
        public string IndoorOutdoor { get; set; } //Indoor/Outdoor
        public ICollection<OlympicTennisRoundViewModel> Rounds { get; set; }
    }

    public class OlympicTennisRoundViewModel
    {
        public int RoundNumber { get; set; }
        public string RoundDesc { get; set; }
        public ICollection<OlympicTennisMatchViewModel> Matches { get; set; }
    }

    public class OlympicTennisMatchViewModel
    {
        public string Side1Player1GivenName { get; set; }
        public string Side1Player1FamilyName { get; set; }
        public string Side1Player1NationCode { get; set; }
        public string Side1Player2GivenName { get; set; }
        public string Side1Player2FamilyName { get; set; }
        public string Side1Player2NationCode { get; set; }
        public string Side2Player1GivenName { get; set; }
        public string Side2Player1FamilyName { get; set; }
        public string Side2Player1NationCode { get; set; }
        public string Side2Player2GivenName { get; set; }
        public string Side2Player2FamilyName { get; set; }
        public string Side2Player2NationCode { get; set; }
        public int WinningSide { get; set; }
        public string Score { get; set; }
    }
}
