using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.Cms
{
    public class TicketsInfoViewModel
    {
        public string Event { get; set; }
        public ICollection<TicketsInfoTieViewModel> Ties { get; set; }
    }

    public class TicketsInfoTieViewModel
    {
        public int? TieId { get; set; }
        public string PublicTieId { get; set; }
        public string Side1Nation { get; set; }
        public string Side1NationCode { get; set; }
        public string Side2Nation { get; set; }
        public string Side2NationCode { get; set; }
        public string OnSaleDate { get; set; }
        public string Website { get; set; }
        public string Telephon { get; set; }
        public string Price { get; set; }
    }
}
