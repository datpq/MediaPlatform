using System;
using System.Xml.Serialization;

namespace ITF.DataServices.SDK.Models.Xml
{
    [Serializable(), XmlRoot("Nation")]
    public class LatestNation
    {
        [XmlAttribute()]
        public string NameEN { get; set; }

        [XmlAttribute()]
        public string NameES { get; set; }

        [XmlAttribute()]
        public string CountryCode { get; set; }
    }
}
