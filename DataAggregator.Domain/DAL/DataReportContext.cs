using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data;
using DataAggregator.Domain.Model.DataReport;
using System;
using System.Collections.Generic;

namespace DataAggregator.Domain.DAL
{
    public class DataReportContext : DbContext
    {
       
        public DataReportContext(string APP)
        {
            Database.SetInitializer<DataReportContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
        }
        public DbSet<Worker> Worker { get; set; }
        public DbSet<WebAggReports> WebAggReports { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<Rep_Type> Rep_Type { get; set; }
        public DbSet<Rep_Param> Rep_Param { get; set; }
    }
}
