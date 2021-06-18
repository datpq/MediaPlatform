using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class CmsDictionary
    {
        public virtual ICollection<CmsLanguageText> CmsLanguageTexts { get; set; }

        [Key]
        public System.Guid Id { get; set; }

        public string Key { get; set; }
        public System.Guid Parent { get; set; }
        public int Pk { get; set; }
    }
}
