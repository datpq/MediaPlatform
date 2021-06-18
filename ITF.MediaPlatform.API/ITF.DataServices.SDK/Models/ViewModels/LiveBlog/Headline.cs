using System.Xml.Serialization;

namespace ITF.DataServices.SDK.Models.ViewModels.LiveBlog
{
    public class Headline
    {
        [XmlText()]
        public string Text { get; set; }
    }
}
