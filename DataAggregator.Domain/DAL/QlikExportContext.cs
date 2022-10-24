using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DataAggregator.Domain.DAL
{
    public class QlikExportContext : DbContext
    {
        public QlikExportContext()
        {
            Database.SetInitializer<QlikExportContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
