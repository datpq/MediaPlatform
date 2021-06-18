namespace ITF.DataServices.SDK.Models.Cms
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("umbracoNode")]
    public class umbracoNode
    {
        public int id { get; set; }

        public bool trashed { get; set; }

        public int parentID { get; set; }

        public int? nodeUser { get; set; }

        public short level { get; set; }

        [Required]
        [StringLength(150)]
        public string path { get; set; }

        public int sortOrder { get; set; }

        public Guid? uniqueID { get; set; }

        [StringLength(255)]
        public string text { get; set; }

        public Guid? nodeObjectType { get; set; }

        public DateTime createDate { get; set; }
    }
}
