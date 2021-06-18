namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class TieResultsCoreViewModel
    {
        public string TieId { get; set; }
        public string S1NationCode { get; set; }
        public int? S1P1Id { get; set; }
        public string S1P1GN { get; set; }
        public string S1P1GNI { get; set; }
        public string S1P1FN { get; set; }
        public int? S1P2Id { get; set; }
        public string S1P2GN { get; set; }
        public string S1P2GNI { get; set; }
        public string S1P2FN { get; set; }
        public string S2NationCode { get; set; }
        public int? S2P1Id { get; set; }
        public string S2P1GN { get; set; }
        public string S2P1GNI { get; set; }
        public string S2P1FN { get; set; }
        public int? S2P2Id { get; set; }
        public string S2P2GN { get; set; }
        public string S2P2GNI { get; set; }
        public string S2P2FN { get; set; }
        public int? WinningSide { get; set; }
        public string Score { get; set; }
        public string ScoreReversed { get; set; }
        public string PlayStatusCode { get; set; }
        public string PlayStatusDesc { get; set; }
    }

    public class TieResultsViewModel : TieResultsCoreViewModel
    {
        public string ScoSet1S1 { get; set; }
        public string ScoSet1S2 { get; set; }
        public string ScoSet1TB { get; set; }
        public string ScoSet2S1 { get; set; }
        public string ScoSet2S2 { get; set; }
        public string ScoSet2TB { get; set; }
        public string ScoSet3S1 { get; set; }
        public string ScoSet3S2 { get; set; }
        public string ScoSet3TB { get; set; }
        public string ScoSet4S1 { get; set; }
        public string ScoSet4S2 { get; set; }
        public string ScoSet4TB { get; set; }
        public string ScoSet5S1 { get; set; }
        public string ScoSet5S2 { get; set; }
        public string ScoSet5TB { get; set; }
    }

    public class TieResultsWebViewModel : TieResultsCoreViewModel
    {
        public int RubberNumber { get; set; }

        public string S1Set1Sco { get; set; } //TODO use int? instead (value of 0 is not currently working in FrontEnd)
        public string S2Set1Sco { get; set; }
        public string S1Set1TBSco { get; set; }
        public string S2Set1TBSco { get; set; }
        public string S1Set2Sco { get; set; }
        public string S2Set2Sco { get; set; }
        public string S1Set2TBSco { get; set; }
        public string S2Set2TBSco { get; set; }
        public string S1Set3Sco { get; set; }
        public string S2Set3Sco { get; set; }
        public string S1Set3TBSco { get; set; }
        public string S2Set3TBSco { get; set; }
        public string S1Set4Sco { get; set; }
        public string S2Set4Sco { get; set; }
        public string S1Set4TBSco { get; set; }
        public string S2Set4TBSco { get; set; }
        public string S1Set5Sco { get; set; }
        public string S2Set5Sco { get; set; }
        public string S1Set5TBSco { get; set; }
        public string S2Set5TBSco { get; set; }

        public string ResultStatusCode { get; set; }
    }
}
