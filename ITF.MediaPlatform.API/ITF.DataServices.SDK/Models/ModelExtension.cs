using System;
using System.Linq;

namespace ITF.DataServices.SDK.Models
{
    public static class ModelExtension
    {
        public static string GetRoundDesc(this int roundNumber)
        {
            var result = roundNumber == 1
                ? "First Round"
                : roundNumber == 2
                    ? "Second Round"
                    : roundNumber == 3 ? "Third Round" : roundNumber == 4 ? "Fourth Round" : null;
            if (result == null)
            {
                throw new ArgumentException($"Can not determine round description with roundNumber={roundNumber}");
            }
            return result;
        }

        public static string GetIndoorOutdoor(this string indoorOutdoorFlag)
        {
            return indoorOutdoorFlag == "I" ? "Indoor" : (indoorOutdoorFlag == "O" ? "Outdoor" : string.Empty);
        }

        public static string GetSurfaceDesc(this string surfaceCode)
        {
            switch (surfaceCode.ToUpper())
            {
                case "A":
                    return "Carpet";
                case "C":
                    return "Clay";
                case "G":
                    return "Grass";
                case "H":
                    return "Hard";
                default:
                    return null;
            }
            
        }

        public static string GetRoundDesc(this string roundCode)
        {
            string result = null;
            switch (roundCode)
            {
                case "1R":
                case "R1":
                    //result = "First Round";
                    result = 1.GetRoundDesc();
                    break;
                case "2R":
                case "R2":
                    //result = "Second Round";
                    result = 2.GetRoundDesc();
                    break;
                case "3R":
                case "R3":
                    //result = "Third Round";
                    result = 3.GetRoundDesc();
                    break;
                case "4R":
                case "R4":
                    //result = "Fourth Round";
                    result = 4.GetRoundDesc();
                    break;
                case "QF":
                    result = "Quarterfinals";
                    break;
                case "SF":
                    result = "Semifinals";
                    break;
                case "FR":
                    result = "Final";
                    break;
            }
            return result;
        }

        public static string GetNationByLanguage(this NationTranslated nationTranslated, Language lang)
        {
            string result;
            switch (lang)
            {
                case Language.En:
                    result = nationTranslated.NationName;
                    break;
                case Language.Es:
                    result = nationTranslated.NationNameSpanish;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
            return result;
        }

        public static string GetNationIByLanguage(this NationTranslated nationTranslated, Language lang)
        {
            string result;
            switch (lang)
            {
                case Language.En:
                    result = nationTranslated.NationName?.Substring(0, 1);
                    break;
                case Language.Es:
                    result = nationTranslated.NationNameSpanish?.Substring(0, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
            return result;
        }

        public static bool ParseBool(this string boolStr)
        {
            return boolStr == "Y";
        }

        public static string GetZoneCode(this string zoneCode, string divisionCode)
        {
            return new[] {"WG", "WG2"}.Contains(divisionCode) ? divisionCode : zoneCode;
        }
    }
}
