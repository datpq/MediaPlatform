using ITF.DataServices.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ITF.DataServices.SDK.Data
{
    public interface ICupDataRepository : IDataRepository
    {
        IEnumerable<T> GetAllSpecific<T>(bool useCache = false) where T : ICupTable;
        IEnumerable<T> GetManySpecific<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : ICupTable;
        T GetSpecific<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : ICupTable;
    }
}
