namespace ITF.DataServices.SDK.Models.Cms
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class CupTickets : ISameStructureTable
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? TieID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(250)]
        public string Title { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(3)]
        public string Nation1 { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(3)]
        public string Nation2 { get; set; }

        [StringLength(50)]
        public string Tickets { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [StringLength(50)]
        public string Tel { get; set; }

        [StringLength(3000)]
        public string Prices { get; set; }

        public bool? Display { get; set; }

        [StringLength(100)]
        public string NationName1 { get; set; }

        [StringLength(100)]
        public string NationName2 { get; set; }

        [StringLength(50)]
        public string PublicTieId { get; set; }
    }

    [Table("CupTicketsDC")]
    public class CupTicketsEnDc : CupTickets
    {
    }

    [Table("CupTicketsDC_ES")]
    public class CupTicketsEsDc : CupTickets
    {
    }

    [Table("CupTicketsFC")]
    public class CupTicketsEnFc : CupTickets
    {
    }

    [Table("CupTicketsFC_ES")]
    public class CupTicketsEsFc : CupTickets
    {
    }
}
