using ITF.DataServices.Authentication.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ITF.DataServices.Authentication.Data
{
    public class AuthenticationDbContext : DbContext
    {
        public AuthenticationDbContext() : base("name=AuthenticationContext")
        {
            Database.SetInitializer<AuthenticationDbContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<CacheConfiguration> CacheConfigurations { get; set; }
    }
}
