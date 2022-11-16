using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data;
using DataAggregator.Domain.Model.Ecom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAggregator.Domain.DAL
{
    public class EcomContext: DbContext
    {
        public DbSet<CoefficientsCount> CoefficientsCount { get; set; }
        public DbSet<CoefficientsPrice> CoefficientsPrice { get; set; }
        public DbSet<RegionalCoefficients> RegionalCoefficients { get; set; }
        public DbSet<Coefficients> Coefficients { get; set; }
        public DbSet<Coefficients_PivotView> Coefficients_PivotView { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public EcomContext(string APP)
        {
            Database.SetInitializer<EcomContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
            Database.CommandTimeout = 0;
        }
        public bool Fill_Table_Coefficient_Default(DateTime Period)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@period", SqlDbType.Date).Value = Period;

                command.CommandText = "EcomNew.Fill_Table_Coefficient_Default";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool EcomRun(DateTime Period)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@period", SqlDbType.Date).Value = Period;
                command.Parameters.Add("@withJob", SqlDbType.Bit).Value = 1;

                command.CommandText = "[EcomNew].[EcomRun]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public async Task<bool> EcomExportSourceRun(DateTime Period)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Year", SqlDbType.Int).Value = Period.Year;
                command.Parameters.Add("@Month", SqlDbType.Int).Value = Period.Month;

                command.CommandText = "[source].[RawDataOnlineOffline]";

                Database.Connection.Open();

                await command.ExecuteNonQueryAsync();
            }
            return true;
        }

        public bool Table_Coefficient_Test_Min_Max()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;


                command.CommandText = "EcomNew.Table_Coefficient_Test_Min_Max";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool Coefficients_from_Excel(string filename, string currentperiod)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@guid", SqlDbType.NVarChar).Value = filename;
                command.Parameters.Add("@currentperiod", SqlDbType.NVarChar).Value = currentperiod;

                command.CommandText = "EcomNew.Coefficients_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
    }
}
