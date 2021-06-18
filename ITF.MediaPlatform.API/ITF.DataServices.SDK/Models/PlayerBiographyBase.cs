using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models
{
    public abstract class PlayerBiographyBase
    {
        public byte? AgeBeganTennis { get; set; }
        public byte? AgeTurnPro { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string BiogNoteCoach { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string BiogNoteRelationship { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string BiographyGeneralNote { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string BiographyTeamCompetitionNote { get; set; }
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string BirthPlace { get; set; }
        public short? BirthYear { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string BrotherNames { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string ChildrenNames { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string CoachName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string FamilyName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string FatherName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string FavouriteSurface { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Gender { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string GivenName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Height { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string MotherName { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string NationalityCode { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string NationalityDesc { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string PersonalNote { get; set; }

        [Key]
        public int PlayerID { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string Residence { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string SisterNames { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string SpouseName { get; set; }
        public byte? TennisCategoryActivePlayingList { get; set; }
        public byte? TennisCategoryPlayedList { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string TennisHands { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Weight { get; set; }
        public short? WinLossCareerDoublesLoss { get; set; }
        public short? WinLossCareerDoublesWalkoverLoss { get; set; }
        public short? WinLossCareerDoublesWalkoverWin { get; set; }
        public short? WinLossCareerDoublesWin { get; set; }
        public short? WinLossCareerSinglesLoss { get; set; }
        public short? WinLossCareerSinglesWalkoverLoss { get; set; }
        public short? WinLossCareerSinglesWalkoverWin { get; set; }
        public short? WinLossCareerSinglesWin { get; set; }
        public short? WinLossCurrentYearDoublesLoss { get; set; }
        public short? WinLossCurrentYearDoublesWalkoverLoss { get; set; }
        public short? WinLossCurrentYearDoublesWalkoverWin { get; set; }
        public short? WinLossCurrentYearDoublesWin { get; set; }
        public short? WinLossCurrentYearSinglesLoss { get; set; }
        public short? WinLossCurrentYearSinglesWalkoverLoss { get; set; }
        public short? WinLossCurrentYearSinglesWalkoverWin { get; set; }
        public short? WinLossCurrentYearSinglesWin { get; set; }
    }
}
