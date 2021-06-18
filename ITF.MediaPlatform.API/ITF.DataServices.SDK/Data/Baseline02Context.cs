using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ITF.DataServices.SDK.Models.Baseline02;

namespace ITF.DataServices.SDK.Data
{
    public class Baseline02Context : DbContext
    {
        public Baseline02Context(string contextName = "Baseline02Context") : base("name=" + contextName)
        {
            Database.SetInitializer<Baseline02Context>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentDetail> TournamentDetails { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<MatchDoubles> MatchDoubles { get; set; }
        public DbSet<MatchSingles> MatchSingles { get; set; }
        public DbSet<MatchPlayer> MatchPlayers { get; set; }
        public DbSet<MatchPlayerEventInfo> MatchPlayerEventInfos { get; set; }
    }
}
