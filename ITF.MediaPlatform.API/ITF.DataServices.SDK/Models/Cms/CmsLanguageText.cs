using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class CmsLanguageText
    {
        [ForeignKey("UniqueId")]
        public virtual CmsDictionary CmsDictionary { get; set; }

        public int LanguageId { get; set; }

        [Key]
        public int Pk { get; set; }

        [ForeignKey("LanguageId")]
        public virtual UmbracoLanguage UmbracoLanguage { get; set; }

        public System.Guid UniqueId { get; set; }
        public string Value { get; set; }
    }
}
