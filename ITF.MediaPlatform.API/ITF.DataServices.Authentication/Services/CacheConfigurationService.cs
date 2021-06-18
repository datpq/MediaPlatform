using System;
using System.Text.RegularExpressions;
using ITF.DataServices.Authentication.Data;
using ITF.DataServices.Authentication.Models;

namespace ITF.DataServices.Authentication.Services
{
    public class CacheConfigurationService : ICacheConfigurationService
    {
        private readonly IDataRepository _dataRepository;
        private const string DefaultKeyPattern = "DEFAULT";

        public CacheConfigurationService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public int GetCacheTimeout(string application, string cachePrefix, string cacheKey = null)
        {
            CacheConfiguration result = null;
            if (cacheKey != null)
            {
                result = _dataRepository.Get<CacheConfiguration>(x => x.Application == application && x.Enabled
                    && x.KeyPattern.StartsWith($"{cachePrefix}.", StringComparison.CurrentCultureIgnoreCase)
                    && Regex.Match(cacheKey, x.KeyPattern, RegexOptions.IgnoreCase).Success, true);
            }
            if (result != null)
            {
                return result.Timeout;
            }
            result = _dataRepository.Get<CacheConfiguration>(
                x => x.Application == application && x.Enabled && x.KeyPattern == cachePrefix, true) ??
                _dataRepository.Get<CacheConfiguration>(
                    x => x.Application == application && x.Enabled && x.KeyPattern == DefaultKeyPattern, true);

            return result.Timeout;
        }
    }
}
