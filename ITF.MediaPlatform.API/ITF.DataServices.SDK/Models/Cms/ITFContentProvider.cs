using System;
using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class ITFContentProvider
    {
        [Key]
        public int Id { get; set; }

        public string ContentProvider { get; set; }
        public int? AssetTypeId { get; set; }
        public DateTime? createDate { get; set; }
        public string Nationality { get; set; }
    }
}
