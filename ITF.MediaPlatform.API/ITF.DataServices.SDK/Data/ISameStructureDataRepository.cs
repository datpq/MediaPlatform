using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ITF.DataServices.SDK.Models;

namespace ITF.DataServices.SDK.Data
{
    public interface ISameStructureDataRepository : IDataRepository
    {
        IEnumerable<T> GetAllSpecific<T>(Language language, DataSource source) where T : ISameStructureTable;
        IEnumerable<T> GetManySpecific<T>(Language language, DataSource source, Expression<Func<T, bool>> where) where T : ISameStructureTable;
    }
}
