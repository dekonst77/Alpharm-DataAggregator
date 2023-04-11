using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DataAggregator.Domain.Model.Distr;
using System.Data.SqlClient;
using System.Data;

namespace DataAggregator.Domain.DAL
{
    public class DistrContext : DbContext
    { 
        public DistrContext(string APP)
        {
            Database.SetInitializer<DistrContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public DbSet<DataSourceType> DataSourceType { get; set; }
        public DbSet<DataSource> DataSource { get; set; }
        public DbSet<Comp> Comp { get; set; }
        public DbSet<CompanyPeriod> CompanyPeriod { get; set; }
        public DbSet<DataType> DataType { get; set; }
        public DbSet<DistributionType> DistributionType { get; set; }
        public DbSet<Distributor> Distributor { get; set; }
        public DbSet<DistributorBranch> DistributorBranch { get; set; }
        public DbSet<FileStatus> FileStatus { get; set; }
        public DbSet<Relation> Relation { get; set; }
        public DbSet<TemplatesMethod> TemplatesMethod { get; set; }
        public DbSet<Templates> Templates { get; set; }
        public DbSet<TemplatesFieldName> TemplatesFieldName { get; set; }
        public DbSet<TemplatesField> TemplatesField { get; set; }
        public DbSet<FileInfo> FileInfo { get; set; }
        public DbSet<Region_Alias> Region_Alias { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Rules_Clients> Rules_Clients { get; set; }
        public DbSet<Supplier> Supplier { get; set; }

        public DbSet<RawData_Out_View> RawData_Out_View { get; set; }
        public DbSet<RawData_Out> RawData_Out { get; set; }
        public DbSet<Project> Project { get; set; }
     

    }
}
