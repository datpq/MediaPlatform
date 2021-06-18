using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class CmsPropertyType
    {
        public string Alias { get; set; }

        public int contentTypeId { get; set; }

        public int dataTypeId { get; set; }

        public string Description { get; set; }

        public string helpText { get; set; }

        [Key]
        public int id { get; set; }

        public bool mandatory { get; set; }
        public string Name { get; set; }
        public int? propertyTypeGroupId { get; set; }
        public int sortOrder { get; set; }
        public string validationRegExp { get; set; }
    }
}
