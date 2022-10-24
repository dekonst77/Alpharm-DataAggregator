using System.Data.Entity;
using DataAggregator.Domain.Model.DataLoader;

namespace DataAggregator.Domain.DAL
{
    public class DataLoaderContext : DbContext
    {
        public DbSet<RawData> RawData { get; set; }

        public DbSet<Task> Task { get; set; }

        public DbSet<TaskParameter> TaskParameter { get; set; }

        public DbSet<TaskLoadStatus> TaskLoadStatuses { get; set; }

        public DbSet<LoadStatus> LoadStatus { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawData>().ToTable("RawData", "DataLoader");
            modelBuilder.Entity<Task>().ToTable("Task", "DataLoader");
            modelBuilder.Entity<TaskParameter>().ToTable("TaskParameter", "DataLoader");
            modelBuilder.Entity<TaskLoadStatus>().ToTable("TaskLoadStatus", "DataLoader");
            modelBuilder.Entity<LoadStatus>().ToTable("LoadStatus", "DataLoader");

            modelBuilder.Entity<RawData>()
                .HasRequired<Task>(d => d.Task)
                .WithMany(t => t.RawData)
                .HasForeignKey(d => d.TaskId);

            modelBuilder.Entity<Task>()
                .HasOptional<Task>(t => t.Parent)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentId);

            modelBuilder.Entity<TaskParameter>()
                .HasRequired<Task>(p => p.Task)
                .WithMany(t => t.TaskParameters)
                .HasForeignKey(p => p.TaskId);

            modelBuilder.Entity<TaskLoadStatus>()
                .HasRequired<Task>(s => s.Task)
                .WithMany(t => t.TaskLoadStatuses)
                .HasForeignKey(s => s.TaskId);

            modelBuilder.Entity<TaskLoadStatus>()
                .HasRequired<LoadStatus>(s => s.LoadStatus)
                .WithMany(s => s.TaskLoadStatuses)
                .HasForeignKey(s => s.LoadStatusId);
        }
    }
}
