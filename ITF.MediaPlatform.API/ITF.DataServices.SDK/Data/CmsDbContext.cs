using ITF.DataServices.SDK.Models.Cms;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ITF.DataServices.SDK.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(string contextName = "UmbracoContext") : base("name=" + contextName)
        {
            Database.SetInitializer<CmsDbContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<ITFRelationships> ItfRelationshipss { get; set; }
        public DbSet<ITFNationBaseline> ItfNationBaselines { get; set; }
        public DbSet<ITFBaselineContent> ItfBaselineContents { get; set; }
        public DbSet<CmsDictionary> CmsDictionaries { get; set; }
        //public DbSet<CmsLanguageText> CmsLanguageTexts { get; set; }
        //public DbSet<UmbracoLanguage> UmbracoLanguages { get; set; }
        public DbSet<CupHeartAwardData> CupHeartAwardDatas { get; set; }
        public DbSet<ITFGallery> ItfGalleries { get; set; }
        public DbSet<ITFHtml> ItfHtmls { get; set; }
        public DbSet<ITFContent> ItfContents { get; set; }
        public DbSet<ITFContentProvider> ItfContentProviders { get; set; }
        public DbSet<ITFMediaDescription> ItfMediaDescriptions { get; set; }
        public DbSet<umbracoNode> UmbracoNodes { get; set; }
        public DbSet<CupTicketsEnDc> CupTicketsEnDcs { get; set; }
        public DbSet<CupTicketsEnFc> CupTicketsEnFcs { get; set; }
        public DbSet<CupTicketsEsDc> CupTicketsEsDcs { get; set; }
        public DbSet<CupTicketsEsFc> CupTicketsEsFcs { get; set; }
    }
}
