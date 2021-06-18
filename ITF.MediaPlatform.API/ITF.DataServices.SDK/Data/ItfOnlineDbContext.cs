using System.Configuration;
using System.Data.Entity;
using System.Linq;
using ITF.DataServices.SDK.Models.ItfOnline;

namespace ITF.DataServices.SDK.Data
{
    public class ItfOnlineDbContext : CommonDbContext
    {
        public ItfOnlineDbContext(string contextName = "ItfOnline1Context") : base(contextName)
        {
            Database.SetInitializer<ItfOnlineDbContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        public static ItfOnlineDbContext GetLiveContext()
        {
            var defaultCtx = new ItfOnlineDbContext();

            var databaseControl = defaultCtx.DatabaseControls.FirstOrDefault();
            if (databaseControl == null)
            {
                throw new ConfigurationErrorsException("DatabaseControl must have at least one record");
            }

            return databaseControl.DBLive.Equals("Baseline_ITFOnline1") ? defaultCtx : new ItfOnlineDbContext("ItfOnline2Context");
        }

        public DbSet<Olympics> Olympics { get; set; }
        public DbSet<Paralympics> Paralympics { get; set; }
        public DbSet<PlayerBiographyMens> PlayerBiographyMens { get; set; }
        public DbSet<PlayerBiographyWomens> PlayerBiographyWomens { get; set; }
        public DbSet<PlayerBiographyWheelchair> PlayerBiographyWheelchairs { get; set; }
        public DbSet<ODFPlayerLookup> OdfPlayerLookups { get; set; }
    }
}
