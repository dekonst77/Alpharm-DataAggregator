using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
    }
}
