using System;
using System.Xml.Serialization;

namespace ITF.DataServices.SDK.Models.Xml
{
    [Serializable(), XmlRoot("Player")]
    public class LatestPlayer
    {
        private string _countryCode;
        private string _doubles;
        private string _familyName;
        private string _gender;
        private string _givenName;
        private string _mixedDoubles;
        private string _quadDoubles;
        private string _quadSingles;
        private string _singles;

        [XmlAttribute()]
        public string CountryCode
        {
            get
            {
                return _countryCode.Trim();
            }
            set
            {
                _countryCode = value;
            }
        }

        [XmlAttribute()]
        public string Doubles
        {
            get
            {
                return _doubles.Trim();
            }
            set
            {
                _doubles = value;
            }
        }

        [XmlAttribute()]
        public string FamilyName
        {
            get
            {
                return _familyName.Trim();
            }
            set
            {
                _familyName = value;
            }
        }

        [XmlAttribute()]
        public string Gender
        {
            get
            {
                return _gender.Trim();
            }
            set
            {
                _gender = value;
            }
        }

        [XmlAttribute()]
        public string GivenName
        {
            get
            {
                return _givenName.Trim();
            }
            set
            {
                _givenName = value;
            }
        }

        [XmlAttribute()]
        public int Id { get; set; }

        [XmlAttribute()]
        public int ITFId { get; set; }

        [XmlAttribute()]
        public string MixedDoubles
        {
            get
            {
                return _mixedDoubles.Trim();
            }
            set
            {
                _mixedDoubles = value;
            }
        }

        [XmlAttribute()]
        public string QuadDoubles
        {
            get
            {
                return _quadDoubles.Trim();
            }
            set
            {
                _quadDoubles = value;
            }
        }

        [XmlAttribute()]
        public string QuadSingles
        {
            get
            {
                return _quadSingles.Trim();
            }
            set
            {
                _quadSingles = value;
            }
        }

        [XmlAttribute()]
        public string Singles
        {
            get
            {
                return _singles.Trim();
            }
            set
            {
                _singles = value;
            }
        }

        [XmlAttribute()]
        public int Year { get; set; }
    }
}
