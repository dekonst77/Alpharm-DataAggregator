using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using DataAggregator.Domain.Model.FtpPoisklekarstv;

namespace DataAggregator.Domain.DAL
{
    public class FtpPoisklekarstvContext : DbContext
    {
        public DbSet<File> File { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<File>().ToTable("File", "service");
        }
    }
}
