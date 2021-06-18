using System;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class NationPlayersWinLossRecords
    {
        public int PlayerId { get; set; }
        public string PlayerGivenName { get; set; }
        public string PlayerFamilyName { get; set; }
        public int SinglesWins { get; set; }
        public int SinglesLosses { get; set; }
        public int DoublesWins { get; set; }
        public int DoublesLosses { get; set; }
        public string SinglesWinLoss { get; set; }
        public string DoublesWinLoss { get; set; }
    }

    public class NationPlayersCareerRecords : NationPlayersWinLossRecords
    {
        public string TotalWinLoss { get; set; }
        public int TotalWin { get; set; }
        public int TotalLoss { get; set; }

        [Obsolete]
        public int SinglesWin { get; set; }
        [Obsolete]
        public int SinglesLoss { get; set; }
        [Obsolete]
        public int DoublesWin { get; set; }
        [Obsolete]
        public int DoublesLoss { get; set; }

        public int TiesPlayed { get; set; }
        public int FirstYearPlayed { get; set; }
        public int YearsPlayed { get; set; }
    }
}
