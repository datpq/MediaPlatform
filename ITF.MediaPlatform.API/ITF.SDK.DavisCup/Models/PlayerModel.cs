namespace ITF.SDK.DavisCup.Models
{
    public class PlayerModel
    {
        public int PlayerId { get; set; }
        public int PlayerInternalId { get; set; }
        public string NationCode { get; set; }
        public string NationNameEN { get; set; }
        public string NationNameES { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string BirthDateEN { get; set; }
        public string BirthDateES { get; set; }
        public string BirthPlace { get; set; }
        public string Residence { get; set; }
        public string Plays { get; set; }
        public string RankSingles { get; set; }
        public string RankDoubles { get; set; }
        public string FirstYearPlayed { get; set; }
        public string TotalNominations { get; set; }
        public string TiesPlayed { get; set; }
        public string WLSingles { get; set; }
        public string WLDoubles { get; set; }
        public string WLTotal { get; set; }
        public string DCCAward { get; set; }
        public string HeadshotImgId { get; set; }
    }
}
