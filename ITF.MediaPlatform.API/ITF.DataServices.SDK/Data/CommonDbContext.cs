using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ITF.DataServices.SDK.Models;

namespace ITF.DataServices.SDK.Data
{
    public class CommonDbContext : DbContext
    {
        public CommonDbContext(string contextName) : base("name=" + contextName)
        {
            Database.SetInitializer<CommonDbContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<NationTranslated> NationTranslateds { get; set; }
        public DbSet<PlayerExternal> PlayerExternals { get; set; }
        public DbSet<DatabaseControl> DatabaseControls { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerActivityMatch> PlayerActivityMatches { get; set; }
        public DbSet<Tie> Ties { get; set; }
        public DbSet<NationHistory> NationHistories { get; set; }
        public DbSet<NationActivityTie> NationActivityTies { get; set; }
        public DbSet<NationGrouping> NationGroupings { get; set; }
        public DbSet<Nation> Nations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRound> EventRounds { get; set; }
        public DbSet<EventNation> EventNations { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentDetail> TournamentDetails { get; set; }
        public DbSet<NationPlayersAgeYoungest> NationPlayersAgeYoungests { get; set; }
        public DbSet<NationPlayersAgeOldest> NationPlayersAgeOldests { get; set; }
        public DbSet<NationLongestRubberInDuration> NationLongestRubberInDurations { get; set; }
        public DbSet<NationLongestTieInDurationHeader> NationLongestTieInDurationHeaders { get; set; }
        public DbSet<NationLongestTieBreakInPoints> NationLongestTieBreakInPoints { get; set; }
        public DbSet<NationLongestFinalSet> NationLongestFinalSets { get; set; }
        public DbSet<NationMostGamesInRubber> NationMostGamesInRubbers { get; set; }
        public DbSet<NationMostGamesInSet> NationMostGamesInSets { get; set; }
        public DbSet<NationMostGamesInTieHeader> NationMostGamesInTieHeaders { get; set; }
        public DbSet<NationMostDecisiveVictoryInTieHeader> NationMostDecisiveVictoryInTieHeaders { get; set; }
        public DbSet<NationLongestWinningRunInTiesHeader> NationLongestWinningRunInTiesHeaders { get; set; }
        public DbSet<NationLongestWinningRunInTiesDetail> NationLongestWinningRunInTiesDetails { get; set; }
        public DbSet<NationComebacksFromTwoNilDownHeader> NationComebacksFromTwoNilDownHeaders { get; set; }
        public DbSet<NationComebacksFromTwoOneDownHeader> NationComebacksFromTwoOneDownHeaders { get; set; }
        public DbSet<NationSquad> NationSquads { get; set; }
    }
}
