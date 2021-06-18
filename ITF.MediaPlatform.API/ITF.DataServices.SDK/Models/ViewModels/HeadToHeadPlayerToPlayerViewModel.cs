using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class HeadToHeadPlayerToPlayerViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerGivenName { get; set; }
        public string PlayerFamilyName { get; set; }
        public string PlayerNationName { get; set; }
        public string PlayerNationCode { get; set; }
        public int OppositionPlayerId { get; set; }
        public string OppositionPlayerGivenName { get; set; }
        public string OppositionPlayerFamilyName { get; set; }
        public string OppositionPlayerNationName { get; set; }
        public string OppositionPlayerNationCode { get; set; }
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
        public int WinSingles { get; set; }
        public int LossSingles { get; set; }
        public int WinDoubles { get; set; }
        public int LossDoubles { get; set; }
        public ICollection<HeadToHeadPlayerToPlayerTie> Ties { get; set; }
    }

    public class HeadToHeadPlayerToPlayerTie
    {
        public string PublicTieId { get; set; }
        public string ResultDesc { get; set; }
        public string Result { get; set; }
        public string Venue { get; set; }
        public int Year { get; set; }
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
        public ICollection<HeadToHeadPlayerToPlayerRubber> Rubbers { get; set; }
    }

    public class HeadToHeadPlayerToPlayerRubber
    {
        public int RubberNumber { get; set; }
        public string ResultCode { get; set; }
        public string Score { get; set; }
        public string MatchTypeCode { get; set; }
        public int? PartnerPlayerId { get; set; }
        public string PartnerPlayerGivenName { get; set; }
        public string PartnerPlayerFamilyName { get; set; }
        public string PartnerPlayerNationCode { get; set; }
        public int? OppositionPlayerId { get; set; }
        public string OppositionPlayerGivenName { get; set; }
        public string OppositionPlayerFamilyName { get; set; }
        public string OppositionPlayerNationCode { get; set; }
        public int? OppositionPartnerPlayerId { get; set; }
        public string OppositionPartnerPlayerGivenName { get; set; }
        public string OppositionPartnerPlayerFamilyName { get; set; }
        public string OppositionPartnerPlayerNationCode { get; set; }
        public int? ScoreSet1Side1 { get; set; }
        public int? ScoreSet1Side2 { get; set; }
        public int? ScoreSet1LosingTB { get; set; }
        public int? ScoreSet2Side1 { get; set; }
        public int? ScoreSet2Side2 { get; set; }
        public int? ScoreSet2LosingTB { get; set; }
        public int? ScoreSet3Side1 { get; set; }
        public int? ScoreSet3Side2 { get; set; }
        public int? ScoreSet3LosingTB { get; set; }
        public int? ScoreSet4Side1 { get; set; }
        public int? ScoreSet4Side2 { get; set; }
        public int? ScoreSet4LosingTB { get; set; }
        public int? ScoreSet5Side1 { get; set; }
        public int? ScoreSet5Side2 { get; set; }
        public int? ScoreSet5LosingTB { get; set; }
    }
}
