using System.Collections.Generic;
using System.Diagnostics;
using ITF.DataServices.SDK.Interfaces;
using System.Runtime.Caching;
using System.Linq;
using System.Runtime.CompilerServices;
using ITF.DataServices.Authentication.Services;

namespace ITF.DataServices.SDK.Services
{
    public abstract class BaseService : IService
    {
        protected readonly ICacheConfigurationService CacheConfigurationService;
        protected readonly MemoryCache MemoryCache = MemoryCache.Default;
        protected readonly string CacheNameSpace;
        private const string CacheApplication = "MediaPlatform";

        public const string McS = "S"; //MatchTypeCode Single
        public const string McD = "D"; //MatchTypeCode Double
        public const string McX = "MX"; //MatchTypeCode Mixed
        public const string EccM = "M"; // Main Draw
        public static readonly List<string> MtcOrders = new List<string> { McS, McD, McX };

        protected BaseService(ICacheConfigurationService cacheConfigurationService)
        {
            CacheConfigurationService = cacheConfigurationService;
            CacheNameSpace = GetType().FullName;
        }

        public virtual void ClearCache(string cachePrefix = null)
        {
            var items = MemoryCache.Where(x => x.Key.StartsWith($"{CacheNameSpace}.{cachePrefix ?? string.Empty}"));

            foreach (var item in items)
            {
                MemoryCache.Remove(item.Key);
            }
        }

        protected string CurrentMethodName
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);

                return sf.GetMethod().Name;
            }
        }

        protected int CurrentLineNumber
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                var st = new StackTrace(true);
                var sf = st.GetFrame(1);

                return sf.GetFileLineNumber();
            }
        }

        protected int GetCacheTimeout(string cachePrefix, string cacheKey = null)
        {
            return CacheConfigurationService.GetCacheTimeout(CacheApplication, cachePrefix, cacheKey);
        }
    }
}
