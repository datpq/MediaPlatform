using System.Configuration;
using System.Data.Entity;
using System.Linq;
using ITF.DataServices.SDK.Models;

namespace ITF.DataServices.SDK.Data
{
    public class FedCupDbContext : CupDbContext<NationRankFedCup, PlayerBiographyFedCup>
    {
        private FedCupDbContext(string contextName = "FedCup1Context") : base(contextName)
        {
            Database.SetInitializer<FedCupDbContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        public override DbSet<PlayerBiographyFedCup> PlayerBiographies { get; set; }
        public override DbSet<NationRankFedCup> NationRanks { get; set; }

        public static FedCupDbContext GetLiveContext()
        {
            var defaultCtx = new FedCupDbContext();

            var databaseControl = defaultCtx.DatabaseControls.FirstOrDefault();
            if (databaseControl == null)
            {
                throw new ConfigurationErrorsException("DatabaseControl must have at least one record");
            }

            return databaseControl.DBLive.Equals("Baseline_FedCup1") ? defaultCtx : new FedCupDbContext("FedCup2Context");
        }
    }
}
