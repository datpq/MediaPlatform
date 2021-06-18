using ITF.DataServices.SDK.Models;
using System.Collections.Generic;
using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq;
using NLog;

namespace ITF.DataServices.SDK.Data
{
    /// <summary>
    /// CupDataRepository is a ICupDataRepository, different databases contain tables with same structure and different names
    /// NationRankDavisCup, NationRankFedCup, ...
    /// </summary>
    /// <typeparam name="TNationRank"></typeparam>
    /// <typeparam name="TPlayerBiographyCup"></typeparam>
    public class CupDataRepository<TNationRank, TPlayerBiographyCup> : DataRepository, ICupDataRepository
        where TNationRank : NationRank
        where TPlayerBiographyCup : PlayerBiographyCup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CupDataRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<T> GetAllSpecific<T>(bool useCache = false) where T : ICupTable
        {
            var cacheKey = $"{DbContext.GetType().Name}.GetAll.{typeof(T).FullName}";
            if (useCache)
            {
                var cacheValue = MemoryCache.Get(cacheKey) as IEnumerable<T>;
                if (cacheValue != null)
                {
                    return cacheValue;
                }
            }
            IEnumerable<T> result;
            if (typeof(T) == typeof(NationRank))
            {
                result = DbContext.Set<TNationRank>() as IEnumerable<T>;
            }
            else if (typeof(T) == typeof(PlayerBiographyCup))
            {
                result = DbContext.Set<TPlayerBiographyCup>() as IEnumerable<T>;
            }
            else
            {
                throw new ArgumentException("Generic type {0} is not supported.", typeof(T).Name);
            }
            if (useCache)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"Added new item in memory cache: {cacheKey}");
                }
                //MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddDays(1));
                var cacheValue = result.ToList().AsQueryable().SetComparer(StringComparison.CurrentCultureIgnoreCase);//ToList here is very important, force executing SQL to retreive data
                MemoryCache.Set(cacheKey, cacheValue, DateTimeOffset.Now.AddDays(1));
                return cacheValue;
            }
            return result;
        }

        public IEnumerable<T> GetManySpecific<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : ICupTable
        {
            if (useCache)
            {
                return GetAllSpecific<T>(true).AsQueryable().Where(where);
            }
            if (typeof(T) == typeof(NationRank))
            {
                return DbContext.Set<TNationRank>().Where(where as Expression<Func<NationRank, bool>>) as IEnumerable<T>;
            }
            if (typeof(T) == typeof(PlayerBiographyCup))
            {
                return DbContext.Set<TPlayerBiographyCup>().Where(where as Expression<Func<PlayerBiographyCup, bool>>) as IEnumerable<T>;
            }
            throw new ArgumentException("Generic type {0} is not supported.", typeof(T).Name);
        }

        public T GetSpecific<T>(Expression<Func<T, bool>> where, bool useCache = false) where T : ICupTable
        {
            return useCache ? GetAllSpecific<T>(true).AsQueryable().Where(@where).FirstOrDefault() : GetManySpecific<T>(@where).FirstOrDefault();
        }
    }
}
