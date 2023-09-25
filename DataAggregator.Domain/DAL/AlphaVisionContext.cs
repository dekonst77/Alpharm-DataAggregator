using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DataAggregator.Domain.Model.Distr;
using System.Data.SqlClient;
using System.Data;
using DataAggregator.Domain.Model.LPU.Alphavision;

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

        public DbSet<AspNetUser> AspNetUsers { get; set; }

        public DbSet<AspNetRole> AspNetRoles { get; set; }

        public DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    }

  
}
