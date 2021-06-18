using System.Collections.Generic;
using ITF.DataServices.SDK.Models.Cms;
using ITF.DataServices.SDK.Models.ViewModels.Circuits;
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using ITF.DataServices.SDK.Models.ViewModels.LiveBlog;

namespace ITF.DataServices.SDK.Interfaces
{
    public interface IOlympicService
    {
        OlympicsPlayersViewModel GetLatestPlayers(string tournamentType, bool useCache = true);
        ICollection<OlympicsPlayersViewModel> GetAllOlympicsPlayers(string tournamentType, bool useCache = true);
        ICollection<PlayerViewModel> GetPlayersForSearch(string tournamentType, bool useCache = true);
        HeadToHeadViewModel GetPlayerHeadToHead(string tournamentType, int playerId, int opponentPlayerId, bool useCache = true);
        ICollection<TournamentViewModel> GetPlayerActivity(string tournamentType, int playerId, bool useCache = true);
        BasePlayerProfileViewModel GetPlayerProfile(string tournamentType, int playerId, bool useCache = true);
        ICollection<OlympicsViewModel> GetAllOlympics(string tournamentType, bool useCache = true);
        DrawsheetViewModel GetDrawsheetByEventId(string tournamentType, int eventId, bool useCache = true);
        ICollection<EventViewModel> GetOlympicEventsByYear(string tournamentType, int year, bool useCache = true);

        LiveBlogDataViewModel GetLiveBlogData(string siteLanguage, string fileName, bool useCache = true);
        Dictionary<string, string> GetTranslationsByIso(string languageIsoCode, bool useCache = true);
        CupHeartAwardData VoteUp(string code, int playerId, string nationCode, string name);

        int? GetPlayerImageAssetId(int playerId, bool useCache = true);
        ICollection<PhotosGalleryViewModel> GetGalleriesByWebScopeId(int webScopeNodeId, bool useCache = true);
        ImageAssetViewModel GetAssetProvider(int assetId, bool useCache = true);
        ICollection<RelatedAssetViewModel> GetRelatedMediaAssets(int assetId, bool useCache = true);
        GalleryAssetViewModel GetGalleryByAssetId(int assetId, int limitImages, bool useCache = true);
        HtmlViewModel GetHtmlByNodeId(int nodeId, bool useCache = true);

        ICollection<OlympicTennisViewModel> GetOlympicTennis(
            Language language = Language.En, DataSource source = DataSource.Itf, bool useCache = true);
        ICollection<GrandSlamViewModel> GetGrandSlam(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Itf, bool useCache = true);
    }
}
