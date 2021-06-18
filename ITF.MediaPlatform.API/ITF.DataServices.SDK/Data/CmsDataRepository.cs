using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ITF.DataServices.SDK.Models;

namespace ITF.DataServices.SDK.Data
{
    /// <summary>
    /// CmsDataRepository is a ISameStructureDataRepository, tables with same structure contained in a same database
    /// CupTicketsDC, CupTicketsDC_ES, CupTicketsFC, CupTicketsFC_ES
    /// </summary>
    public class CmsDataRepository : DataRepository, ISameStructureDataRepository
    {
        public CmsDataRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<T> GetAllSpecific<T>(Language language, DataSource source) where T : ISameStructureTable
        {
            var setMethod = DbContext.GetType().GetMethod("Set", new Type[] { }).MakeGenericMethod(Type.GetType(typeof(T).FullName + language + source));
            var result = setMethod.Invoke(DbContext, new object[] { }) as IEnumerable<T>;
            return result;
        }

        public IEnumerable<T> GetManySpecific<T>(Language language, DataSource source, Expression<Func<T, bool>> where) where T : ISameStructureTable
        {
            var setMethod = DbContext.GetType().GetMethod("Set", new Type[] { }).MakeGenericMethod(Type.GetType(typeof(T).FullName + language + source));
            var result = (setMethod.Invoke(DbContext, new object[] { }) as IQueryable<T>)?.Where(where) as IEnumerable<T>;
            return result;
        }
    }
}
