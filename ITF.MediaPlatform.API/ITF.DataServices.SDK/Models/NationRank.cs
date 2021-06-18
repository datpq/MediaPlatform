namespace ITF.DataServices.SDK.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public abstract class NationRank : ICupTable
    {
        [Key]
        [Column(Order = 0, TypeName = "VARCHAR")]
        [StringLength(3)]
        public string NationCode { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string NationName { get; set; }

        public DateTime? RankDate { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Rank { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string RankEqualFlag { get; set; }

        public DateTime? RankPrevDate { get; set; }

        public short? RankPrev { get; set; }

        [StringLength(1)]
        [Column(TypeName = "VARCHAR")]
        public string RankEqualPrevFlag { get; set; }

        public int? TotalTiesPlayed { get; set; }

        public decimal? RankingPoints { get; set; }
    }

    [Table("NationRankDavisCup")]
    public class NationRankDavisCup : NationRank
    {
    }

    [Table("NationRankFedCup")]
    public class NationRankFedCup : NationRank
    {
    }
}
