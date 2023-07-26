using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using DataAggregator.Domain.Model.DataAggregator;
using DataAggregator.Domain.Model.Project;

namespace DataAggregator.Domain.DAL
{
    public class DataAggregatorContext : DbContext
    {
        public DbSet<ErrorLog> ErrorLog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DataAggregatorContext(string APP)
        {
            Database.SetInitializer<DataAggregatorContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
        }
        public DbSet<History> History { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectType> ProjectType { get; set; }
        public DbSet<StepStatus> StepStatus { get; set; }
        public DbSet<StepTemplate> StepTemplate { get; set; }
        public DbSet<Steps> Steps { get; set; }

        public DbSet<UserViewAll> UserViewAll { get; set; }
        public DbSet<NotificationGroups> NotificationGroups { get; set; }
        public DbSet<NotificationGroupUsers> NotificationGroupUsers { get; set; }
    }
}