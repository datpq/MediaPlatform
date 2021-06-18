using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;
using ITF.DataServices.SDK.Models.Xml;
using NLog;

namespace ITF.DataServices.SDK.Data
{
    public class OlympicsXmlRepository : XmlRepository
    {
        private readonly XmlDocument _nationDocument = new XmlDocument();
        private readonly XmlDocument _playerDocument = new XmlDocument();
        private readonly string _nationXPath;
        private readonly string _playerXPath;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OlympicsXmlRepository(string nationXmlFile, string nationXPath,
            string playerXmlFile, string playerXPath)
        {
            _nationXPath = nationXPath;
            _playerXPath = playerXPath;

            _nationDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nationXmlFile)); // Can not use HttpContext.Current.Server.MapPath
            _playerDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, playerXmlFile));
        }

        private static IEnumerable<T> GetAll<T>(XmlNode xmlDocument, string xPath)
        {
            var xmlNodeList = xmlDocument.SelectNodes(xPath);
            if (xmlNodeList == null)
            {
                throw new ArgumentNullException($"Nothing found at path: {xPath}");
            }
            var serializer = new XmlSerializer(typeof(T));
            var result = new List<T>();
            xmlNodeList.Cast<XmlNode>().ToList().ForEach(x =>
            {
                var item = (T)serializer.Deserialize(XmlReader.Create(new StringReader(x.OuterXml)));
                result.Add(item);
            });
            return result;
        }

        public override T Deserialize<T>(string xmlFilePath)
        {
            try
            {
                var xmlData = new XmlDocument();
                //var physicalPath = HttpContext.Current != null
                //    ? HttpContext.Current.Server.MapPath(xmlFilePath)
                //    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFilePath);
                var physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFilePath);
                xmlData.Load(physicalPath);

                var serializer = new XmlSerializer(typeof(T));

                var data = (T) serializer.Deserialize(XmlReader.Create(new StringReader(xmlData.OuterXml)));

                return data;
            }
            catch (IOException e)
            {
                Logger.Warn(e, $"Error when accessing file {xmlFilePath}");
                return null;
            }
        }

        public override IEnumerable<T> GetAll<T>(bool useCache = false)
        {
            if (typeof(T) == typeof(LatestNation))
            {
                return GetAll<T>(_nationDocument, _nationXPath);
            }
            if (typeof(T) == typeof(LatestPlayer))
            {
                return GetAll<T>(_playerDocument, _playerXPath);
            }
            throw new ArgumentException("Generic type {0} is not supported.", typeof(T).Name);
        }

        public override T Get<T>(Expression<Func<T, bool>> where, bool useCache = false)
        {
            return GetAll<T>().AsQueryable().Where(where).FirstOrDefault();
        }

        public override IEnumerable<T> GetMany<T>(Expression<Func<T, bool>> where, bool useCache = false)
        {
            return GetAll<T>().AsQueryable().Where(where);
        }
    }
}
