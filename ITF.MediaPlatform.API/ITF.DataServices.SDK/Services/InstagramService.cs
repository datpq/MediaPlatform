using System;
using System.Diagnostics;
using System.Net.Http;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models.Media;
using Newtonsoft.Json;
using NLog;

namespace ITF.DataServices.SDK.Services
{
    public class InstagramService : BaseService, IInstagramService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly HttpClient _httpClient;

        public InstagramService(HttpClient httpClient, ICacheConfigurationService cacheConfigurationService) : base(cacheConfigurationService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.instagram.com/");
        }

        public Instagram GetProfileData(string profile, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{profile}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as Instagram;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var httpResponse = _httpClient.GetAsync($"{_httpClient.BaseAddress}{profile}/media").Result;
                var stringResponse = httpResponse.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Instagram>(stringResponse);
                if (result.items != null && result.items.Count > 10)
                {
                    result.items = result.items.GetRange(0, 10);
                }

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }
    }
}
