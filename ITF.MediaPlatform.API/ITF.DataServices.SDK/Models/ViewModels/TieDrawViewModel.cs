using System;
using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class TieDrawViewModel
    {
        public string Name { get; set; }
        public string Side1NationName { get; set; }
        [Obsolete]
        public string Side1NationNameES { get; set; }
        public string Side1NationCode { get; set; }
        public int Side1H2HWin { get; set; }
        public int? Side1Seeding { get; set; }
        public bool IsSide1Hosting { get; set; }
        public string Side2NationName { get; set; }
        [Obsolete]
        public string Side2NationNameES { get; set; }
        public string Side2NationCode { get; set; }
        public int Side2H2HWin { get; set; }
        public int? Side2Seeding { get; set; }
        public bool IsSide2Hosting { get; set; }
        public bool IsCogByLot { get; set; }
        public string Venue { get; set; }
        public int? Side1Score { get; set; }
        public int? Side2Score { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PublicTieId { get; set; }
        public string PlayStatus { get; set; }
        public bool IsRubberInPlay { get; set; }
        public bool IsBye { get; set; }
    }

    public class TieDrawFullViewModel
    {
        public string Name { get; set; }
        public string Side1NationName { get; set; }
        public string Side1NationCode { get; set; }
        public string Side2NationName { get; set; }
        public string Side2NationCode { get; set; }
        public string Venue { get; set; }
        public int? Side1Score { get; set; }
        public int? Side2Score { get; set; }
        public string PublicTieId { get; set; }

        public string HostNationName { get; set; }
        public string HostNationCode { get; set; }
        public string Surface { get; set; }
        public string IndoorOutdoor { get; set; }
        public int? WinningSide { get; set; }
        public string Score { get; set; }
        public string Result { get; set; }
        public int TieId { get; set; }
        public string Date { get; set; }
        public ICollection<ResultMatchViewModel> Matches { get; set; }
        public ICollection<ResultTeamViewModel> Teams { get; set; }
    }

    public class ResultMatchViewModel
    {
        public string Side1Player1GivenName { get; set; }
        public string Side1Player1FamilyName { get; set; }
        public string Side1Player2GivenName { get; set; }
        public string Side1Player2FamilyName { get; set; }
        public string Side2Player1GivenName { get; set; }
        public string Side2Player1FamilyName { get; set; }
        public string Side2Player2GivenName { get; set; }
        public string Side2Player2FamilyName { get; set; }
        public string WinningSide { get; set; }
        public string Score { get; set; }
    }

    public class ResultTeamViewModel
    {
        public string NationCode { get; set; }
        public string Captain { get; set; }
        public ICollection<ResultsPlayerViewModel> Players { get; set; }
    }

    public class ResultsPlayerViewModel
    {
        public string PlayerGivenName { get; set; }
        public string PlayerFamilyName { get; set; }
        public int? PlayerId { get; set; }
    }
}
