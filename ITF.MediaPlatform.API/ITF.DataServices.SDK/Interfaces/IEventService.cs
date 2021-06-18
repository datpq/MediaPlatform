using System.Collections.Generic;
using ITF.DataServices.SDK.Models.ViewModels;

namespace ITF.DataServices.SDK.Interfaces
{
    public interface IEventService
    {
        ICollection<EventYearViewModel> GetEventYears(string section, string subSection,
            DataSource source = DataSource.Dc, bool useCache = true);
        DrawSheetViewModel GetDrawSheet(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ResultsByYearViewModel GetResultsByYear(int year,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<RoundRobinEventViewModel> GetRoundRobinEvents(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<RoundRobinNominationViewModel> GetRoundRobinNominations(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        TieDetailsWebViewModel GetTieDetailsWeb(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        TieDetailsAppViewModel GetTieDetails(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        TieDetailsViewModel GetTieDetails(int tieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<TieNominationViewModel> GetTieNominations(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<TieDetailsWebViewModel> GetNodeRelatedTieDetails(int nodeId, Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<TieResultsWebViewModel> GetTieResultsWeb(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<TieResultsViewModel> GetTieResults(int tieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        TieViewModel GetTie(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<TournamentViewModel> GetTournaments(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);
        ICollection<TieDetailsViewModel> SearchTies(string searchText1, string searchText2,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true);

    }
}
