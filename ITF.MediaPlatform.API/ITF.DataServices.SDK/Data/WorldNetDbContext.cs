using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ITF.DataServices.SDK.Models.WorldNet;

namespace ITF.DataServices.SDK.Data
{
    public class WorldNetDbContext : DbContext
    {
        public WorldNetDbContext(string contextName = "WorldNetContext") : base("name=" + contextName)
        {
            Database.SetInitializer<WorldNetDbContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Baseline360Export> Baseline360Exports { get; set; }
    }
}
