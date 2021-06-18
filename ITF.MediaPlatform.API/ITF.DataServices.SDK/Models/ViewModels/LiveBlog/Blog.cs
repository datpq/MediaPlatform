using System.Xml.Serialization;

namespace ITF.DataServices.SDK.Models.ViewModels.LiveBlog
{
    public class Blog
    {
        [XmlAttribute("aid")]
        public string AudioId { get; set; }

        [XmlAttribute("bg")]
        public string Background { get; set; }

        [XmlAttribute("del")]
        public bool Deleted { get; set; }

        [XmlAttribute("ed")]
        public bool Edit { get; set; }

        [XmlAttribute("edt")]
        public string EditTime { get; set; }

        [XmlAttribute("gmt")]
        public string Gmt { get; set; }

        [XmlText()]
        public string Html { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("iid")]
        public string ImageId { get; set; }

        [XmlAttribute("tlk")]
        public string Link { get; set; }

        [XmlAttribute("tle")]
        public string Title { get; set; }

        [XmlAttribute("ytid")]
        public string YoutubeId { get; set; }
    }
}
