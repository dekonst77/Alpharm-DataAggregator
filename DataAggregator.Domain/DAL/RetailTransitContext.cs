using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DataAggregator.Domain.Model.RetailTransit;

namespace DataAggregator.Domain.DAL
{
    public class RetailTransitContext : DbContext
    {
        public DbSet<RiglaFile> RiglaFile { get; set; }

        public DbSet<RiglaLog> RiglaLog { get; set; }

        public RetailTransitContext()
        {
            Database.SetInitializer<RetailTransitContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
