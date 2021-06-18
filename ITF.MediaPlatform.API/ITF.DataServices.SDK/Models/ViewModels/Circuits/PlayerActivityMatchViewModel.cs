using TypeLite;

namespace ITF.DataServices.SDK.Models.ViewModels.Circuits
{
    [TsClass]
    public class PlayerActivityMatchViewModel
    {
        public string OpponentPartnerPlayerFamilyName { get; set; }
        public string OpponentPartnerPlayerGivenName { get; set; }
        public int OpponentPartnerPlayerID { get; set; }
        public string OpponentPartnerPlayerNationalityCode { get; set; }
        public string OpponentPlayerFamilyName { get; set; }
        public string OpponentPlayerGivenName { get; set; }
        public int OpponentPlayerID { get; set; }
        public string OpponentPlayerNationalityCode { get; set; }
        public string PartnerPlayerFamilyName { get; set; }
        public string PartnerPlayerGivenName { get; set; }
        public int PartnerPlayerID { get; set; }
        public string PartnerPlayerNationalityCode { get; set; }
        public string PlayerEntryClassificationCode { get; set; }
        public string ResultCode { get; set; }
        public string RoundCode { get; set; }
        public int RoundNumber { get; set; }
        public string Score { get; set; }

        public string TournamentName { get; set; }
    }
}
