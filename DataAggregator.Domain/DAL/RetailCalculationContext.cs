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
    }
}
