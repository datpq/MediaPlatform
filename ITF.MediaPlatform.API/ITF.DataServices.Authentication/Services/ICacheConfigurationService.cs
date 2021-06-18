namespace ITF.DataServices.Authentication.Services
{
    public interface ICacheConfigurationService
    {
        int GetCacheTimeout(string application, string cachePrefix, string cacheKey = null);
    }
}
