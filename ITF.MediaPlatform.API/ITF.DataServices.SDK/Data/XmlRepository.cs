using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ITF.DataServices.SDK.Data
{
    public abstract class XmlRepository : IXmlDataRepository
    {
        public abstract T Deserialize<T>(string xmlFilePath) where T : class;
        public abstract IEnumerable<T> GetAll<T>(bool useCache = false) where T : class;
        public abstract T Get<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : class;
        public abstract IEnumerable<T> GetMany<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : class;

        public void Add<T>(T entity) where T : class
        {
            throw new InvalidOperationException("Invalid method call for XmlRepository");
        }

        public void Delete<T>(T entity) where T : class
        {
            throw new InvalidOperationException("Invalid method call for XmlRepository");
        }

        public void Delete<T>(Expression<Func<T, bool>> where) where T : class
        {
            throw new InvalidOperationException("Invalid method call for XmlRepository");
        }

        public void Commit()
        {
            throw new InvalidOperationException("Invalid method call for XmlRepository");
        }

        public void EnableDebug(Action<string> action)
        {
            throw new InvalidOperationException("Invalid method call for XmlRepository");
        }

        public void DisableDebug()
        {
            throw new InvalidOperationException("Invalid method call for XmlRepository");
        }
    }
}
