using ITF.DataServices.Authentication;
using ITF.DataServices.SDK;
using Ninject;

namespace ITF.MediaPlatform.API.App_Start
{
    public static class MyAppStart
    {
        public static void RegisterServices(IKernel kernel)
        {
            AuthenticationAppStart.RegisterServices(kernel);
            DataServicesAppStart.RegisterServices(kernel);

            //var playerService = kernel.Get<IPlayerService>();
        }

        public static void RegisterMappings()
        {
            //AutoMapper.Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<PlayerModel, PlayerViewModel>();
            //});
        }
    }
}