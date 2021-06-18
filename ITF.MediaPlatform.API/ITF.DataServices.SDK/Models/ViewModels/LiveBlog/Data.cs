using System.Collections.Generic;
using System.Xml.Serialization;

namespace ITF.DataServices.SDK.Models.ViewModels.LiveBlog
{
    [XmlRoot("data")]
    public class Data
    {
        [XmlElement("blogs")]
        public Blogs Blogs { get; set; }

        [XmlAttribute("date")]
        public string Date { get; set; }

        [XmlAttribute("editor")]
        public string Editor { get; set; }

        [XmlElement("headlines")]
        public Headlines Headlines { get; set; }
    }

    public class Headlines
    {
        [XmlElement("headline")]
        public List<Headline> Headline { get; set; }
    }

    public class Blogs
    {
        [XmlElement("blog")]
        public List<Blog> Blog { get; set; }
    }
}
