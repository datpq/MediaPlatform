using System.Configuration;
using System.Data.Entity;
using System.Linq;
using ITF.DataServices.SDK.Models;

namespace ITF.DataServices.SDK.Data
{
    public class DavisCupDbContext : CupDbContext<NationRankDavisCup, PlayerBiographyDavisCup>
    {
        private DavisCupDbContext(string contextName = "DavisCup1Context") : base(contextName)
        {
            Database.SetInitializer<DavisCupDbContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        public override DbSet<PlayerBiographyDavisCup> PlayerBiographies { get; set; }
        public override DbSet<NationRankDavisCup> NationRanks { get; set; }

        public static DavisCupDbContext GetLiveContext()
        {
            var defaultCtx = new DavisCupDbContext();

            var databaseControl = defaultCtx.DatabaseControls.FirstOrDefault();
            if (databaseControl == null)
            {
                throw new ConfigurationErrorsException("DatabaseControl must have at least one record");
            }

            return databaseControl.DBLive.Equals("Baseline_DavisCup1") ? defaultCtx : new DavisCupDbContext("DavisCup2Context");
        }
    }
}
