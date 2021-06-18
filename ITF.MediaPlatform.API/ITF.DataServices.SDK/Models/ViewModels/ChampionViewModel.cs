namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class ChampionViewModel
    {
        public string TieId { get; set; }
        public int Year { get; set; }
        public string ChampionNationName { get; set; }
        public string ChampionNationCode { get; set; }
        public int? ChampionScore { get; set; }
        public string FinalistNationName { get; set; }
        public string FinalistNationCode { get; set; }
        public int? FinalistScore { get; set; }
        public string Venue { get; set; }
    }
}
