using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data;
using DataAggregator.Domain.Model.DataReport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DataAggregator.Domain.DAL
{
    public class DataReportContext : DbContext
    {

        public DataReportContext(string APP)
        {
            Database.SetInitializer<DataReportContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
            Database.Log = (query) => Debug.Write(query);
        }

        public DbSet<Worker> Worker { get; set; }
        public DbSet<WebAggReports> WebAggReports { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<Rep_Type> Rep_Type { get; set; }
        public DbSet<Rep_Param> Rep_Param { get; set; }

        public DbSet<ReportsLog> ReportsLog { get; set; }
        public DbSet<ReportsLogView> ReportsLogView { get; set; }

        public DataTable GetDataTableFromQuery(string query, string server, string APP, bool snapshotOn)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = new SqlConnection("Persist Security Info=true;Server=" + server + ";Database=tempdb;Integrated Security=SSPI;APP=" + APP);

                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                SqlTransaction sqlTran;
                //по умолчанию все запросы с использованием SNAPSHOT
                if (snapshotOn)
                {
                    sqlTran = command.Connection.BeginTransaction(IsolationLevel.Snapshot);
                    command.Transaction = sqlTran;
                }
                else
                {
                    sqlTran = command.Connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command.Transaction = sqlTran;
                }

                command.CommandTimeout = 0;
                command.CommandText = query;

                var tbl = new DataTable("tbl");
                try
                {
                    tbl.Load(command.ExecuteReader());
                    if (command.Transaction != null)
                        command.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (command.Transaction != null)
                            command.Transaction.Rollback();
                    }
                    catch
                    {
                    }
                    throw ex;
                }
                return tbl;
            }
        }

        public ReportsLog LogStart(WebAggReports report, List<cField> filter, Guid userId)
        {
            var log = new ReportsLog()
            {
                ReportId = report.Id,
                Filters = filter == null ? String.Empty : String.Join("", filter?.ToArray().Select(x => JsonConvert.SerializeObject(x))),
                UserId = userId,
                DateStart = DateTime.Now,
                StatusId = 0
            };
            this.ReportsLog.Add(log);
            this.SaveChanges();
            return log;
        }

        public void LogEnd(ReportsLog reportLog, int statusId)
        {
            var log = this.ReportsLog.Where(x => x.Id == reportLog.Id).Single();
            if (log != null)
            {
                log.StatusId = statusId;
                log.DateEnd = DateTime.Now;
                this.SaveChanges();
            }
        }
    }

}
