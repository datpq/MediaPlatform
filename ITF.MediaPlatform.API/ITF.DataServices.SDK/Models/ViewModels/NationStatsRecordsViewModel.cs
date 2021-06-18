using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class NationStatsRecordsViewModel
    {
        public int YoungestPlayerId { get; set; }
        public string YoungestPlayerName { get; set; }
        public string YoungestPlayerDOBEN { get; set; }
        public string YoungestPlayerDOBES { get; set; }
        public int YoungestPlayerAgeYearsPlayed { get; set; }
        public int YoungestPlayerAgeDaysPlayed { get; set; }
        public string YoungestPlayerPlayedDateEN { get; set; }
        public string YoungestPlayerPlayedDateES { get; set; }
        public int OldestPlayerId { get; set; }
        public string OldestPlayerName { get; set; }
        public string OldestPlayerDOBEN { get; set; }
        public string OldestPlayerDOBES { get; set; }
        public int OldestPlayerAgeYearsPlayed { get; set; }
        public int OldestPlayerAgeDaysPlayed { get; set; }
        public string OldestPlayerPlayedDateEN { get; set; }
        public string OldestPlayerPlayedDateES { get; set; }
        public string LongestRubberDurationTime { get; set; }
        public string LongestRubberDurationTieId { get; set; }
        public string LongestRubberDurationResult { get; set; }
        public string LongestTieDurationTime { get; set; }
        public string LongestTieDurationTieId { get; set; }
        public string LongestTieDurationResult { get; set; }
        public ICollection<NationStatsLongestTieBreak> LongestTieBreak { get; set; }
        public ICollection<NationStatsLongestFinalSet> LongestFinalSet { get; set; }
        public ICollection<NationStatsMostGamesInRubber> MostGamesInRubber { get; set; }
        public ICollection<NationStatsMostGamesInSet> MostGamesInSet { get; set; }
        public ICollection<NationStatsMostGamesInTie> MostGamesInTie { get; set; }
        public ICollection<NationStatsMostDecisiveVictoryInTie> MostDecisiveVictoryInTie { get; set; }
        public int? LongestWinRunNumber { get; set; }
        public ICollection<NationStatsLongestWinRun> LongestWinRun { get; set; }
        public ICollection<NationStatsComebackTwoNilDown> ComebackTwoNilDown { get; set; }
        public ICollection<NationStatsComebackTwoOneDown> ComebackTwoOneDown { get; set; }
    }

    public class NationStatsLongestTieBreak
    {
        public string TieId { get; set; }
        public int TotalPoints { get; set; }
        public string TieBreakScore { get; set; }
        public string RubberResult { get; set; }
    }

    public class NationStatsLongestFinalSet
    {
        public string TieId { get; set; }
        public int TotalGames { get; set; }
        public string SetScore { get; set; }
        public string RubberResult { get; set; }
    }

    public class NationStatsMostGamesInRubber
    {
        public string TieId { get; set; }
        public int TotalGames { get; set; }
        public string RubberScore { get; set; }
        public string RubberResult { get; set; }
    }

    public class NationStatsMostGamesInSet
    {
        public string TieId { get; set; }
        public int TotalGames { get; set; }
        public string RubberScore { get; set; }
        public string RubberResult { get; set; }
    }

    public class NationStatsMostGamesInTie
    {
        public string TieId { get; set; }
        public int TotalGames { get; set; }
        public string TieResult { get; set; }
    }

    public class NationStatsMostDecisiveVictoryInTie
    {
        public string TieId { get; set; }
        public string SetsWinLoss { get; set; }
        public string GamesWinLoss { get; set; }
        public string TieResult { get; set; }
    }

    public class NationStatsLongestWinRun
    {
        public string TieId { get; set; }
        public string TieResult { get; set; }
        public string TieStartDate { get; set; }
        public string TieEndDate { get; set; }
        public string TieEvent { get; set; }
    }

    public class NationStatsComebackTwoNilDown
    {
        public string TieId { get; set; }
        public string TieResult { get; set; }
        public string TieStartDate { get; set; }
        public string TieEndDate { get; set; }
        public string TieEvent { get; set; }
    }

    public class NationStatsComebackTwoOneDown
    {
        public string TieId { get; set; }
        public string TieResult { get; set; }
        public string TieStartDate { get; set; }
        public string TieEndDate { get; set; }
        public string TieEvent { get; set; }
    }
}
