using ITF.DataServices.SDK.Models.ViewModels;
using System.Collections.Generic;
using ITF.DataServices.SDK.Models.ViewModels.Btd;

namespace ITF.DataServices.SDK.Interfaces
{
    public interface INationService
    {
        NationViewModelOld GetNation(string nationCode, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        HeadToHeadNationToNationViewModel GetHeadToHeadNationToNation(string nationCode, string opponentNationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<NationCoreViewModelOld> GetNations(Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<NationWinLossRecordsViewModel> GetNationWinLossRecords(string nationCode, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<NationPlayersWinLossRecords> GetNationPlayersWinLossRecords(string nationCode, int year,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<NationPlayersCareerRecords> GetNationPlayersCareerRecords(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        NationStatsRecordsViewModel GetNationStatsRecords(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<NationRankingViewModel> GetNationRankings(int top = 0, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        NationAllTimeRecordsViewModel GetNationAllTimeRecords(string nationCode, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<NationsGroupViewModel> GetNationsGroup(int year, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        NationProfileWebViewModelOld GetNationProfileWeb(string nationCode, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<PlayerViewModelCore> GetNationPlayers(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<PlayerViewModelCore> GetNationRecentPlayers(string nationCode, int recentYears,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<ChampionViewModel> GetChampions(Language language = Language.En, DataSource source = DataSource.Dc,
            bool useCache = true);
        ICollection<NationCoreViewModel> GetNodeRelatedNations(int nodeId, Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<NationCoreViewModel> SearchNations(string searchText, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true); 

        BtdNationViewModel GetBtdNation(string nationCode, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        BtdMyTeamViewModel GetBtdMyTeam(string nationCode, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
    }
}
