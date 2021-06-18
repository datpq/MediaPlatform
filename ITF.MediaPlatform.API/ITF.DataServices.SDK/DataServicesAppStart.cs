using ITF.DataServices.SDK.Data;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models;
using ITF.DataServices.SDK.Models.Cms;
using ITF.DataServices.SDK.Models.ViewModels;
using ITF.DataServices.SDK.Models.ViewModels.Circuits;
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using ITF.DataServices.SDK.Services;
using Ninject;
using Ninject.Web.Common;
using MatchViewModel = ITF.DataServices.SDK.Models.ViewModels.Circuits.MatchViewModel;

namespace ITF.DataServices.SDK
{
    public static class DataServicesAppStart
    {
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ICupDataRepository>().ToMethod(x =>
                new CupDataRepository<NationRankDavisCup, PlayerBiographyDavisCup>(
                    DavisCupDbContext.GetLiveContext())).InRequestScope().Named("DavisCupRepo");
            kernel.Bind<ICupDataRepository>().ToMethod(x =>
                new CupDataRepository<NationRankFedCup, PlayerBiographyFedCup>(
                    FedCupDbContext.GetLiveContext())).InRequestScope().Named("FedCupRepo");
            //kernel.Bind<IDataRepository>().ToMethod(x => new DataRepository(new CmsDbContext())).Named("CmsRepo");
            kernel.Bind<ISameStructureDataRepository>().ToMethod(x => new CmsDataRepository(new CmsDbContext())).InRequestScope().Named("CmsRepo");
            kernel.Bind<IDataRepository>().ToMethod(x => new DataRepository(new WorldNetDbContext())).InRequestScope().Named("WorldNetRepo");
            kernel.Bind<IDataRepository>().ToMethod(x =>
                new DataRepository(ItfOnlineDbContext.GetLiveContext())).InRequestScope().Named("ItfOnlineRepo");
            kernel.Bind<IDataRepository>().ToMethod(x =>
                new DataRepository(new Baseline02Context())).InRequestScope().Named("Baseline02Repo");
            kernel.Bind<IXmlDataRepository>().ToConstant(
                new OlympicsXmlRepository("App_Data/nations.xml", "/Nations/Nation", "App_Data/players_OLY.xml", "/Players/Player"))
                .Named("OlympicsXmlOlyRepo");
            kernel.Bind<IXmlDataRepository>().ToConstant(
                new OlympicsXmlRepository("App_Data/nations.xml", "/Nations/Nation", "App_Data/players_PLY.xml", "/Players/Player"))
                .Named("OlympicsXmlPlyRepo");
            kernel.Bind<IPlayerService>().To<PlayerService>().InRequestScope();
            kernel.Bind<INationService>().To<NationService>().InRequestScope();
            kernel.Bind<IEventService>().To<EventService>().InRequestScope();
            kernel.Bind<ICmsService>().To<CmsService>().InRequestScope();
            kernel.Bind<IOlympicService>().To<OlympicService>().InRequestScope();
            kernel.Bind<IInstagramService>().To<InstagramService>().InRequestScope();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;
                cfg.CreateMap<PlayerActivityMatch, PlayerActivityMatchViewModel>()
                    .ForMember(m => m.PartnerPlayerID, opt => opt.MapFrom(src => src.PartnerDataExchangePlayerId))
                    .ForMember(m => m.OpponentPlayerID, opt => opt.MapFrom(src => src.OpponentDataExchangePlayerId))
                    .ForMember(m => m.OpponentPartnerPlayerID, opt => opt.MapFrom(src => src.OpponentPartnerDataExchangePlayerId))
                    ;
                cfg.CreateMap<Match, MatchViewModel>()
                    .ForMember(m => m.Side1Player1ID, opt => opt.MapFrom(src => src.Side1Player1DataExchangePlayerId))
                    .ForMember(m => m.Side1Player2ID, opt => opt.MapFrom(src => src.Side1Player2DataExchangePlayerId))
                    .ForMember(m => m.Side2Player1ID, opt => opt.MapFrom(src => src.Side2Player1DataExchangePlayerId))
                    .ForMember(m => m.Side2Player2ID, opt => opt.MapFrom(src => src.Side2Player2DataExchangePlayerId))
                    ;
                cfg.CreateMap<ITFGallery, PhotosGalleryViewModel>()
                    .ForMember(m => m.TitleEn, opt => opt.MapFrom(src => src.Name))
                    .ForMember(m => m.TitleEs, opt => opt.MapFrom(src => src.DescriptionText))
                    .ForMember(m => m.GalleryId, opt => opt.MapFrom(src => src.NodeId))
                    ;
                cfg.CreateMap<NationProfileWebViewModelOld, NationProfileWebViewModel>();
                cfg.CreateMap<NationViewModelOld, NationViewModel>();
                cfg.CreateMap<TieViewModelOld, TieViewModel>();
                cfg.CreateMap<PlayerViewModelCoreOld, PlayerViewModelCore>();
                cfg.CreateMap<PlayerViewModelCore, PlayerViewModelCoreCore>();
            });
        }
    }
}
