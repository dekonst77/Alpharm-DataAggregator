using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DataAggregator.Domain.Model.OFD;
using System.Data.SqlClient;
using System.Data;
using DataAggregator.Domain.Model.EtalonPrice;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataAggregator.Domain.DAL
{
    public class OFDContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public DbSet<ofdFilenames_View> ofdFilenames_Views { get; set; }
        public DbSet<Aggregated_Period> Aggregated_Period { get; set; }
        public DbSet<Period_4SC> Period_4SC { get; set; }
        public DbSet<Week_Periods> Week_Periods { get; set; }
        
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<List> List { get; set; }
        public DbSet<Brick_L3_all> Brick_L3_all { get; set; }
        //public DbSet<licenses_adress> licenses_adress { get; set; }
        public DbSet<PriceEtalon> PriceEtalon { get; set; }

        public DbSet<PriceEtalonView> PriceEtalonView { get; set; }
        public DbSet<Classifier_ExternalView> Classifier_ExternalView { get; set; }

        public DbSet<Aggregated_All> Aggregated_All { get; set; }
        public DbSet<Data_All_4SC> Data_All_4SC { get; set; }
        public DbSet<Agreement> Agreement_All { get; set; }
        public DbSet<Classifier> AgreementClassifiers { get; set; }

        public DbSet<PriceCurrentView> PriceCurrentView { get; set; }
        public DbSet<PriceCurrent> PriceCurrent { get; set; }

        public DbSet<PriceCurrentView_v2> PriceCurrentView_v2 { get; set; }
        public DbSet<PriceCurrent_v2> PriceCurrent_v2 { get; set; }
        public DbSet<CommentStatuses> CommentStatuses { get; set; }
        public DbSet<MainData> MainData { get; set; }

        public OFDContext(string APP)
        {
            Database.SetInitializer<OFDContext>(null);
            Database.Connection.ConnectionString += "APP="+ APP;//Чтобы триггер увидел, кто меняет
            Database.Log = (query) => Debug.Write(query);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public bool File_Delete_To_Update(string path, string userName, bool withFile)
        {

            //@path nvarchar(255),@userName nvarchar(255),@withFile
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@path", SqlDbType.NVarChar).Value = path;
                command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;
                command.Parameters.Add("@withFile", SqlDbType.Bit).Value = withFile;

                    command.CommandText = "dbo.File_Delete_To_Update";

                Database.Connection.Open();

                    command.ExecuteNonQuery();
                }
            return true;
        }

        public string StatusCalcCurrentPrice()
        {
            return DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(this, "PriceCurrent_Calculated", Domain.Model.ControlALG.ControlALG.JobStartAction.info);
        }

        public string RunCalcCurrentPrice()
        {
            return DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(this, "PriceCurrent_Calculated", Domain.Model.ControlALG.ControlALG.JobStartAction.start);
        }

        public void CurrentPriceCopyToEtalonPrice(int year, int month, Guid userId)
        {
         
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                command.CommandText = "[EtalonPrice].[CopyToEtalonPrice]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            
        }

        public void CurrentPriceCopyToEtalonPrice_v2(int year, int month, Guid userId)
        {

            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                command.CommandText = "[EtalonPrice].[CopyToEtalonPrice_v2]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }

        }

        public string RunCalcCurrentPrice_v2()
        {
            return DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(this, "PriceCurrent_Calculated_v2", Domain.Model.ControlALG.ControlALG.JobStartAction.start);
        }

        public bool report_List_Get(int Id, string email, int supplierID, string text, System.DateTime period_start, System.DateTime period_end, string Brick_L3)
        {
            //exec [report].[List.Get] 2,'s.starinov@alpharm.ru'
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                command.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                command.Parameters.Add("@supplierId", SqlDbType.Int).Value = supplierID;
                command.Parameters.Add("@text", SqlDbType.NVarChar).Value = text;
                command.Parameters.Add("@period_start", SqlDbType.Date).Value = period_start;
                command.Parameters.Add("@period_end", SqlDbType.Date).Value = period_end;
                command.Parameters.Add("@Brick_L3", SqlDbType.NVarChar).Value = Brick_L3;

                command.CommandText = "[report].[List.Get]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public IEnumerable<Aggregated_All> GetAggsearch_Result(int SupplierId, DateTime periodStart, DateTime periodEnd, int BrickId, long? ClassifierId = null)
        {
            return Database.SqlQuery<Aggregated_All>("[dbo].[GetAggsearch] @ClassifierId, @SupplierId, @periodStart, @periodEnd, @BrickId",
                new SqlParameter { ParameterName = "@ClassifierId", SqlDbType = SqlDbType.BigInt, Value = (object)ClassifierId ?? DBNull.Value, IsNullable = true },
                new SqlParameter { ParameterName = "@SupplierId", SqlDbType = SqlDbType.Int, Value = SupplierId },
                new SqlParameter { ParameterName = "@periodStart", SqlDbType = SqlDbType.Date, Value = periodStart },
                new SqlParameter { ParameterName = "@periodEnd", SqlDbType = SqlDbType.Date, Value = periodEnd },
                new SqlParameter { ParameterName = "@BrickId", SqlDbType = SqlDbType.Int, Value = BrickId }
                );
        }

        public void ImportAgreements_from_Excel(string filename, bool force = false)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;
                command.Parameters.Add("@force", SqlDbType.Bit).Value = force;

                command.CommandText = "4SC.ImportAgreements_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Импорт эталонных цен
        /// </summary>
        /// <param name="month">месяц</param>
        /// <param name="year">год</param>
        /// <param name="UserId">UserId</param>
        /// <param name="filename">файл</param>
        public void ImportPriceCurrent_from_Excel(int month, int year, Guid UserId, string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = UserId;
                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "[EtalonPrice].[ImportPriceCurrent_from_Excel]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Вывод эталонных цен для блока эталонных цен new 
        /// </summary>
        /// <param name="month">месяц</param>
        /// <param name="year">год</param>
        public IEnumerable<EtalonPriceViewData_SP_Result> ViewData_SP(int year, int month, decimal? devPercent = null, string searchText = null)
        {
            return Database.SqlQuery<EtalonPriceViewData_SP_Result>("[EtalonPrice].[ViewData_SP] @PeriodShort, @DevPercent, @SearchText",
               new SqlParameter { ParameterName = "@PeriodShort", SqlDbType = SqlDbType.Int, Value = year * 100 + month },
               new SqlParameter { ParameterName = "@DevPercent", SqlDbType = SqlDbType.Decimal, Value = (object)devPercent ?? DBNull.Value },
               new SqlParameter { ParameterName = "@SearchText", SqlDbType = SqlDbType.VarChar, Value = (object)searchText ?? DBNull.Value }
               );
        }

        /// <summary>
        /// Перезагрузка всех источников и SKU для блока эталонных цен new 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        public void ReloadAllData_SP(int year, int month)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@PeriodShort", SqlDbType = SqlDbType.Int, Value = year * 100 + month } 
            }
            .Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [EtalonPrice].[ReloadAllData_SP] @PeriodShort", parameters);
        }
    }
}
