using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ITF.DataServices.SDK
{
    public static class Constants
    {
        public const string DefaultLanguage = "en";
        public const string DefaultSource = "dc";

        #region extension utilities

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static int OrderOf<T>(this List<T> lst, T item)
        {
            var idx = lst.IndexOf(item);
            return idx < 0 ? lst.Count : idx;
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<int, T> action)
        {
            // argument null checking omitted
            var i = 0;
            foreach (var item in sequence)
            {
                action(i, item);
                i++;
            }
        }

        public static string ToStringStandardFormat(this TimeSpan ts)
        {
            return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds/10:00}";
        }

        public static string FormatShortByLanguage(this DateTime? dt, Language lang)
        {
            return dt?.FormatShortByLanguage(lang);
        }

        public static string FormatMediumByLanguage(this DateTime? dt, Language lang)
        {
            return dt?.FormatMediumByLanguage(lang);
        }

        public static string FormatLongByLanguage(this DateTime? dt, Language lang)
        {
            return dt?.FormatLongByLanguage(lang);
        }

        public static string FormatShortByLanguage(this DateTime dt, Language lang)
        {
            switch (lang)
            {
                case Language.En:
                    return dt.ToString("dd MMM", new CultureInfo("en-GB"));
                case Language.Es:
                    return dt.ToString("dd MMM", new CultureInfo("es-ES"));

                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
        }

        public static string FormatMediumByLanguage(this DateTime dt, Language lang)
        {
            switch (lang)
            {
                case Language.En:
                    return dt.ToString("dd MMM yyyy", new CultureInfo("en-GB"));
                case Language.Es:
                    return dt.ToString("dd MMM yyyy", new CultureInfo("es-ES"));

                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
        }

        public static string FormatLongByLanguage(this DateTime dt, Language lang)
        {
            switch (lang)
            {
                case Language.En:
                    return dt.ToString("dd MMMM yyyy", new CultureInfo("en-GB"));
                case Language.Es:
                    return dt.ToString("dd MMMM yyyy", new CultureInfo("es-ES"));

                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, null);
            }
        }

        public static bool SqlNotEquals(this string x, string y)
        {
            return x != y && x != null;
        }

        public static DataSource ParseDataSource(this string source)
        {
            DataSource dataSource;
            if (!Enum.TryParse(source, true, out dataSource))
            {
                dataSource = (DataSource)Enum.Parse(typeof(DataSource), DefaultSource, true);
            }
            return dataSource;
        }

        public static Language ParseLanguage(this string language)
        {
            Language lang;
            if (!Enum.TryParse(language, true, out lang))
            {
                lang = (Language)Enum.Parse(typeof(Language), DefaultLanguage, true);
            }
            return lang;
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        #endregion
    }

    public enum DataSource
    {
        Dc,
        Fc,
        Itf
    }

    public enum Language
    {
        En,
        Es
    }
}
