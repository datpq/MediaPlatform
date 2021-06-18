using System.Collections.Generic;
using ITF.DataServices.SDK.Models.ViewModels;
using ITF.DataServices.SDK.Models.ViewModels.Btd;

namespace ITF.DataServices.SDK.Interfaces
{
    public interface IPlayerService : IService
    {
        PlayerViewModelCoreOld GetPlayerCore(int playerId, Language language = Language.En, DataSource source = DataSource.Itf, bool useCache = true);
        PlayerViewModel GetPlayer(int playerId, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        PlayerActivityViewModelOld GetPlayerActivity(int playerId, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        HeadToHeadPlayerToPlayerViewModel GetHeadToHeadPlayerToPlayer(int playerId, int opponentPlayerId, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        HeadToHeadPlayerToNationViewModel GetHeadToHeadPlayerToNation(int playerId, string opponentNationCode, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        BtdPlayerViewModel GetBtdPlayer(int playerId, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<PlayerViewModelCoreOld> GetFeaturedPlayers(int cmsNodeId, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<PlayerViewModelCoreOld> GetCommitmentAwardEligiblePlayers(Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<PlayerViewModelCoreOld> GetCommitmentAwardOneTieToPlayPlayers(Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<PlayerViewModelCore> GetPlayersFromTie(string publicTieId, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<PlayerViewModelCore> SearchPlayers(string searchText, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        int? GetHeadshotImgId(int internalPlayerId);
    }
}