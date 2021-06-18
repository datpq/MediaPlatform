using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models
{
    public abstract class PlayerBiography : PlayerBiographyBase
    {
        [Column(TypeName = "VARCHAR")]
        public string RankCurrentSinglesGrandPrix { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankCurrentSinglesRollover { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankCurrentDoublesGrandPrix { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankCurrentDoublesRollover { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankCareerHighSinglesGrandPrix { get; set; }
        public DateTime? RankCareerHighSinglesGrandPrixDate { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankCareerHighSinglesRollover { get; set; }
        public DateTime? RankCareerHighSinglesRolloverDate { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankCareerHighDoublesGrandPrix { get; set; }
        public DateTime? RankCareerHighDoublesGrandPrixDate { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankCareerHighDoublesRollover { get; set; }
        public DateTime? RankCareerHighDoublesRolloverDate { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankYearEndSinglesGrandPrix { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankYearEndSinglesRollover { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankYearEndDoublesGrandPrix { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string RankYearEndDoublesRollover { get; set; }
    }

    public abstract class PlayerBiographyCup : PlayerBiography, ICupTable
    {
        public virtual short? WinLossCurrentYearSinglesCupWin { get; set; }
        public virtual short? WinLossCurrentYearSinglesCupLoss { get; set; }
        public virtual short? WinLossCurrentYearSinglesCupWalkoverWin { get; set; }
        public virtual short? WinLossCurrentYearSinglesCupWalkoverLoss { get; set; }

        public virtual short? WinLossCurrentYearDoublesCupWin { get; set; }
        public virtual short? WinLossCurrentYearDoublesCupLoss { get; set; }
        public virtual short? WinLossCurrentYearDoublesCupWalkoverWin { get; set; }
        public virtual short? WinLossCurrentYearDoublesCupWalkoverLoss { get; set; }

        public virtual short? WinLossCareerSinglesCupWin { get; set; }
        public virtual short? WinLossCareerSinglesCupLoss { get; set; }
        public virtual short? WinLossCareerSinglesCupWalkoverWin { get; set; }
        public virtual short? WinLossCareerSinglesCupWalkoverLoss { get; set; }

        public virtual short? WinLossCareerDoublesCupWin { get; set; }
        public virtual short? WinLossCareerDoublesCupLoss { get; set; }
        public virtual short? WinLossCareerDoublesCupWalkoverWin { get; set; }
        public virtual short? WinLossCareerDoublesCupWalkoverLoss { get; set; }

        public virtual short? TotalCareerCupWeeks { get; set; }

        public int? DataExchangePlayerId { get; set; }
    }

    public class PlayerBiographyDavisCup : PlayerBiographyCup
    {
        [Column("WinLossCurrentYearSinglesDavisCupWin")]
        public override short? WinLossCurrentYearSinglesCupWin { get; set; }
        [Column("WinLossCurrentYearSinglesDavisCupLoss")]
        public override short? WinLossCurrentYearSinglesCupLoss { get; set; }
        [Column("WinLossCurrentYearSinglesDavisCupWalkoverWin")]
        public override short? WinLossCurrentYearSinglesCupWalkoverWin { get; set; }
        [Column("WinLossCurrentYearSinglesDavisCupWalkoverLoss")]
        public override short? WinLossCurrentYearSinglesCupWalkoverLoss { get; set; }

        [Column("WinLossCurrentYearDoublesDavisCupWin")]
        public override short? WinLossCurrentYearDoublesCupWin { get; set; }
        [Column("WinLossCurrentYearDoublesDavisCupLoss")]
        public override short? WinLossCurrentYearDoublesCupLoss { get; set; }
        [Column("WinLossCurrentYearDoublesDavisCupWalkoverWin")]
        public override short? WinLossCurrentYearDoublesCupWalkoverWin { get; set; }
        [Column("WinLossCurrentYearDoublesDavisCupWalkoverLoss")]
        public override short? WinLossCurrentYearDoublesCupWalkoverLoss { get; set; }

        [Column("WinLossCareerSinglesDavisCupWin")]
        public override short? WinLossCareerSinglesCupWin { get; set; }
        [Column("WinLossCareerSinglesDavisCupLoss")]
        public override short? WinLossCareerSinglesCupLoss { get; set; }
        [Column("WinLossCareerSinglesDavisCupWalkoverWin")]
        public override short? WinLossCareerSinglesCupWalkoverWin { get; set; }
        [Column("WinLossCareerSinglesDavisCupWalkoverLoss")]
        public override short? WinLossCareerSinglesCupWalkoverLoss { get; set; }

        [Column("WinLossCareerDoublesDavisCupWin")]
        public override short? WinLossCareerDoublesCupWin { get; set; }
        [Column("WinLossCareerDoublesDavisCupLoss")]
        public override short? WinLossCareerDoublesCupLoss { get; set; }
        [Column("WinLossCareerDoublesDavisCupWalkoverWin")]
        public override short? WinLossCareerDoublesCupWalkoverWin { get; set; }
        [Column("WinLossCareerDoublesDavisCupWalkoverLoss")]
        public override short? WinLossCareerDoublesCupWalkoverLoss { get; set; }

        [Column("TotalCareerDavisCupWeeks")]
        public override short? TotalCareerCupWeeks { get; set; }
    }

    public class PlayerBiographyFedCup : PlayerBiographyCup
    {
        [Column("WinLossCurrentYearSinglesFedCupWin")]
        public override short? WinLossCurrentYearSinglesCupWin { get; set; }
        [Column("WinLossCurrentYearSinglesFedCupLoss")]
        public override short? WinLossCurrentYearSinglesCupLoss { get; set; }
        [Column("WinLossCurrentYearSinglesFedCupWalkoverWin")]
        public override short? WinLossCurrentYearSinglesCupWalkoverWin { get; set; }
        [Column("WinLossCurrentYearSinglesFedCupWalkoverLoss")]
        public override short? WinLossCurrentYearSinglesCupWalkoverLoss { get; set; }

        [Column("WinLossCurrentYearDoublesFedCupWin")]
        public override short? WinLossCurrentYearDoublesCupWin { get; set; }
        [Column("WinLossCurrentYearDoublesFedCupLoss")]
        public override short? WinLossCurrentYearDoublesCupLoss { get; set; }
        [Column("WinLossCurrentYearDoublesFedCupWalkoverWin")]
        public override short? WinLossCurrentYearDoublesCupWalkoverWin { get; set; }
        [Column("WinLossCurrentYearDoublesFedCupWalkoverLoss")]
        public override short? WinLossCurrentYearDoublesCupWalkoverLoss { get; set; }

        [Column("WinLossCareerSinglesFedCupWin")]
        public override short? WinLossCareerSinglesCupWin { get; set; }
        [Column("WinLossCareerSinglesFedCupLoss")]
        public override short? WinLossCareerSinglesCupLoss { get; set; }
        [Column("WinLossCareerSinglesFedCupWalkoverWin")]
        public override short? WinLossCareerSinglesCupWalkoverWin { get; set; }
        [Column("WinLossCareerSinglesFedCupWalkoverLoss")]
        public override short? WinLossCareerSinglesCupWalkoverLoss { get; set; }

        [Column("WinLossCareerDoublesFedCupWin")]
        public override short? WinLossCareerDoublesCupWin { get; set; }
        [Column("WinLossCareerDoublesFedCupLoss")]
        public override short? WinLossCareerDoublesCupLoss { get; set; }
        [Column("WinLossCareerDoublesFedCupWalkoverWin")]
        public override short? WinLossCareerDoublesCupWalkoverWin { get; set; }
        [Column("WinLossCareerDoublesFedCupWalkoverLoss")]
        public override short? WinLossCareerDoublesCupWalkoverLoss { get; set; }

        [Column("TotalCareerFedCupWeeks")]
        public override short? TotalCareerCupWeeks { get; set; }
    }
}
