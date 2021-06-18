using System.Data.Entity;
using ITF.DataServices.SDK.Models;
using ITF.DataServices.SDK.Models.Cup;

namespace ITF.DataServices.SDK.Data
{
    public class CupDbContext<TNationRank, TPlayerBiographyCup> : CommonDbContext
        where TNationRank : NationRank
        where TPlayerBiographyCup : PlayerBiographyCup
    {
        public CupDbContext(string contextName) : base(contextName)
        {
            Database.SetInitializer<CupDbContext<TNationRank, TPlayerBiographyCup>>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<TNationRank> NationRanks { get; set; }
        public virtual DbSet<TPlayerBiographyCup> PlayerBiographies { get; set; }
        public DbSet<PlayerTeamCompetitionParticipationSummary> PlayerTeamCompetitionParticipationSummaries { get; set; }
        public DbSet<EventPlayOffsDisplayOrder> EventPlayOffsDisplayOrders { get; set; }
        public DbSet<TeamNomination> TeamNominations { get; set; }
        public DbSet<TeamNominationPlayer> TeamNominationPlayers { get; set; }
    }
}