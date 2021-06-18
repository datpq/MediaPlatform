using ITF.DataServices.Authentication.Data;
using ITF.DataServices.Authentication.Services;
using Ninject;
using Ninject.Web.Common;

namespace ITF.DataServices.Authentication
{
    public static class AuthenticationAppStart
    {
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IDataRepository>().To<DataRepository>().InRequestScope();
            kernel.Bind<ITokenService>().To<TokenService>().InRequestScope();
            kernel.Bind<IUserService>().To<UserService>().InRequestScope();
            kernel.Bind<ICacheConfigurationService>().To<CacheConfigurationService>().InRequestScope();
        }
    }
}
