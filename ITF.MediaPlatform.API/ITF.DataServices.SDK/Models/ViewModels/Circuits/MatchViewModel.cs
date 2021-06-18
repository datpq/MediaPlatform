using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    [TsClass]
    public class MatchViewModel
    {
        public int DrawsheetPositionMatch { get; set; }
        public string DrawsheetRoundCode { get; set; }
        public string DrawsheetRoundDesc { get; set; }
        public byte? DrawsheetRoundNumber { get; set; }
        public int EventID { get; set; }
        public int MatchID { get; set; }
        public string Score { get; set; }
        public string Side1Player1FamilyName { get; set; }
        public string Side1Player1GivenName { get; set; }
        public int? Side1Player1ID { get; set; }
        public string Side1Player1NationalityCode { get; set; }
        public string Side1Player2FamilyName { get; set; }
        public string Side1Player2GivenName { get; set; }
        public int? Side1Player2ID { get; set; }
        public string Side1Player2NationalityCode { get; set; }
        public int? Side1Seeding { get; set; }
        public string Side1TieNationCode { get; set; }
        public string Side2Player1FamilyName { get; set; }
        public string Side2Player1GivenName { get; set; }
        public int? Side2Player1ID { get; set; }
        public string Side2Player1NationalityCode { get; set; }
        public string Side2Player2FamilyName { get; set; }
        public string Side2Player2GivenName { get; set; }
        public int? Side2Player2ID { get; set; }
        public string Side2Player2NationalityCode { get; set; }
        public short? Side2Seeding { get; set; }
        public string Side2TieNationCode { get; set; }
        public int? TieID { get; set; }
        public int TournamentID { get; set; }
        public int WinningSide { get; set; }
    }
}
