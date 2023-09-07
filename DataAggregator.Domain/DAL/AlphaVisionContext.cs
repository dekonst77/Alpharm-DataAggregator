using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DataAggregator.Domain.Model.Distr;
using System.Data.SqlClient;
using System.Data;

namespace DataAggregator.Domain.DAL
{
    public class AlphaVisionContext : DbContext
    { 
        public AlphaVisionContext(string APP)
        {
            Database.SetInitializer<AlphaVisionContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP; //Чтобы триггер увидел, кто меняет
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public DbSet<DataSourceType> DataSourceType { get; set; }
       
     

    }
}
