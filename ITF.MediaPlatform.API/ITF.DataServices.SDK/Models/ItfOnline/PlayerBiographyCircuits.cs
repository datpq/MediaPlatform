using System;

namespace ITF.DataServices.SDK.Models.ItfOnline
{
    public class PlayerBiographyMens : PlayerBiography
    {
    }

    public class PlayerBiographyWomens : PlayerBiography
    {
    }

    public class PlayerBiographyWheelchair : PlayerBiographyBase
    {
        public byte? AgeBeganWCT { get; set; }

        public string RankCareerHighDoubles { get; set; }
        public DateTime? RankCareerHighDoublesDate { get; set; }
        public string RankCareerHighDoublesJunior { get; set; }
        public DateTime? RankCareerHighDoublesJuniorDate { get; set; }
        public string RankCareerHighDoublesQuad { get; set; }
        public DateTime? RankCareerHighDoublesQuadDate { get; set; }
        public string RankCareerHighSingles { get; set; }
        public DateTime? RankCareerHighSinglesDate { get; set; }
        public string RankCareerHighSinglesJunior { get; set; }
        public DateTime? RankCareerHighSinglesJuniorDate { get; set; }
        public string RankCareerHighSinglesQuad { get; set; }
        public DateTime? RankCareerHighSinglesQuadDate { get; set; }
        public string RankCurrentDoubles { get; set; }
        public string RankCurrentDoublesJunior { get; set; }
        public string RankCurrentDoublesQuad { get; set; }
        public string RankCurrentSingles { get; set; }
        public string RankCurrentSinglesJunior { get; set; }
        public string RankCurrentSinglesQuad { get; set; }
        public string RankYearEndDoubles { get; set; }
        public string RankYearEndDoublesJunior { get; set; }
        public string RankYearEndDoublesQuad { get; set; }
        public string RankYearEndSingles { get; set; }
        public string RankYearEndSinglesJunior { get; set; }
        public string RankYearEndSinglesQuad { get; set; }
        public string RegisterQuadFlag { get; set; }
        public string RegisterQuadSuspendedFlag { get; set; }
        public string WheelchairManufacturer { get; set; }
        public int? WheelchairManufacturerID { get; set; }
        public short? WinLossCareerDoublesQuadLoss { get; set; }
        public short? WinLossCareerDoublesQuadWalkoverLoss { get; set; }
        public short? WinLossCareerDoublesQuadWalkoverWin { get; set; }
        public short? WinLossCareerDoublesQuadWin { get; set; }
        public short? WinLossCareerSinglesQuadLoss { get; set; }
        public short? WinLossCareerSinglesQuadWalkoverLoss { get; set; }
        public short? WinLossCareerSinglesQuadWalkoverWin { get; set; }
        public short? WinLossCareerSinglesQuadWin { get; set; }
        public short? WinLossCurrentYearDoublesQuadLoss { get; set; }
        public short? WinLossCurrentYearDoublesQuadWalkoverLoss { get; set; }
        public short? WinLossCurrentYearDoublesQuadWalkoverWin { get; set; }
        public short? WinLossCurrentYearDoublesQuadWin { get; set; }
        public short? WinLossCurrentYearSinglesQuadLoss { get; set; }
        public short? WinLossCurrentYearSinglesQuadWalkoverLoss { get; set; }
        public short? WinLossCurrentYearSinglesQuadWalkoverWin { get; set; }
        public short? WinLossCurrentYearSinglesQuadWin { get; set; }
    }
}
