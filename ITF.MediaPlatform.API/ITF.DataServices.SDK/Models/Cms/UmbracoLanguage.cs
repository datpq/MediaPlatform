using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class UmbracoLanguage
    {
        public virtual ICollection<CmsLanguageText> CmsLanguageTexts { get; set; }

        [Key]
        public int Id { get; set; }

        public string LanguageCultureName { get; set; }
        public string LanguageISOCode { get; set; }
    }
}
