using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.RetailCalculation;

namespace DataAggregator.Domain.DAL
{
    public class RetailCalculationContext : DbContext
    {
        #region Process

        public DbSet<Info> ProcessInfo { get; set; }

        public DbSet<Launcher> ProcessLauncher { get; set; }

        public DbSet<Process> ProcessProcess { get; set; }

        public DbSet<Status> ProcessStatus { get; set; }

        public DbSet<Log> CalculationLog { get; set; }

        public DbSet<TargetPharmacyWithoutAverageIn> TargetPharmacyWithoutAverageIn { get; set; }
        public DbSet<TargetPharmacyWithoutAverageOut> TargetPharmacyWithoutAverageOut { get; set; }

        #endregion

        public RetailCalculationContext()
        {
            Database.SetInitializer<RetailContext>(null);
            Database.CommandTimeout = 6000;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }

        public bool RulesCommit(DateTime PeriodFrom, DateTime PeriodTo)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@YearStart", SqlDbType.Int).Value = PeriodFrom.Year;
                command.Parameters.Add("@MonthStart", SqlDbType.Int).Value = PeriodFrom.Month;
                command.Parameters.Add("@YearEnd", SqlDbType.Int).Value = PeriodTo.Year;
                command.Parameters.Add("@MonthEnd", SqlDbType.Int).Value = PeriodTo.Month;

                command.CommandText = "[process].[RulesCommit]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool HistoryCalculation(DateTime PeriodFrom, DateTime PeriodTo)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@YearStart", SqlDbType.Int).Value = PeriodFrom.Year;
                command.Parameters.Add("@MonthStart", SqlDbType.Int).Value = PeriodFrom.Month;
                command.Parameters.Add("@YearEnd", SqlDbType.Int).Value = PeriodTo.Year;
                command.Parameters.Add("@MonthEnd", SqlDbType.Int).Value = PeriodTo.Month;

                command.CommandText = "[process].[HistoryCalculation]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public void ImportTargetPharmacyWithoutAverage_from_Excel(int month, int year, string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "ImportTargetPharmacyWithoutAverage_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}
