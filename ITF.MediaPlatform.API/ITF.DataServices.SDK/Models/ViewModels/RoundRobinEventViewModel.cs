using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class RoundRobinEventViewModel
    {
        public int EventId { get; set; }
        public string Venue { get; set; }
        public string Surface { get; set; }
        public string SubGroupCode { get; set; }
        public string DrawsheetStructureCode { get; set; }
        public string EventClassificationDesc { get; set; }
        public int EventOrder { get; set; }
        public ICollection<RoundRobinEventResultRrViewModel> ResultsRR { get; set; }
        public ICollection<RoundRobinTieViewModel> ResultsPO { get; set; }
    }

    public class RoundRobinTieViewModel
    {
        public string PublicTieId { get; set; }
        public string Side1NationName { get; set; }
        public string Side1NationCode { get; set; }
        public int Side1Score { get; set; }
        public string Side2NationName { get; set; }
        public string Side2NationCode { get; set; }
        public int Side2Score { get; set; }
        public string PlayStatus { get; set; }
        public bool IsInProgress { get; set; }
        public string Date { get; set; }
    }

    public class RoundRobinEventResultRrViewModel
    {
        public int EventId { get; set; }
        public string NationName { get; set; }
        public string NationCode { get; set; }
        public int? Seeding { get; set; }
        public int? DrawPosition { get; set; }
        public int TiePlayed { get; set; }
        public int TieWon { get; set; }
        public int TieLost { get; set; }
        public string Rubbers { get; set; }
        public ICollection<RoundRobinTieViewModel> Ties { get; set; }
    }
}
