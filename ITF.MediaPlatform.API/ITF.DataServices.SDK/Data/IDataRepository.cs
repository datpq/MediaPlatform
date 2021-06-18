﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ITF.DataServices.SDK.Data
{
    public interface IDataRepository
    {
        IEnumerable<T> GetAll<T>(bool useCache = false) where T : class;
        T Get<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : class;
        IEnumerable<T> GetMany<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : class;
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Delete<T>(Expression<Func<T, bool>> where) where T : class;
        void Commit();
        void EnableDebug(Action<string> action);
        void DisableDebug();
    }
}
