using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using DataAggregator.Domain.Model.GRLS;

namespace DataAggregator.Domain.DAL
{
    public class GrlsContext : DbContext
    {
        public DbSet<Page> GrlsPage { get; set; }
        public DbSet<Drug> GrlsDrug { get; set; }
        public DbSet<ProductionStage> GrlsProductionStage { get; set; }
        public DbSet<RegistrationCertificate> GrlsRegistrationCertificate { get; set; }
        public DbSet<DrugInfo> GrlsDrugInfo { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public void UpdateAnalyze(long id, int analyzeId, string errorMessage)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GrlsContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
                    command.Parameters.Add("@AnalyzeId", SqlDbType.Int).Value = analyzeId;
                    command.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar).Value = errorMessage;

                    command.CommandText = "dbo.UpdateAnalyze";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
