using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DataAggregator.Domain.Model.Alphavision;

namespace DataAggregator.Domain.DAL
{
    public class AlphaVisionContext : DbContext
    {
        public DbSet<AspNetUser> AspNetUsers { get; set; }

        public DbSet<AspNetUsers_View> AspNetUsers_View { get; set; }

        public DbSet<AspNetRole> AspNetRoles { get; set; }

        public DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

        public DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Post> Posts { get; set; }

        public AlphaVisionContext(string APP)
        {
            Database.SetInitializer<AlphaVisionContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP; //Чтобы триггер увидел, кто меняет
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<AspNetUserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            //modelBuilder.Entity<AspNetUserRole>().HasRequired(ur => ur.Role)
            //        .WithMany(r => r.UserRoles)
            //        .HasForeignKey(ur => ur.RoleId);
            //modelBuilder.Entity<AspNetUserRole>().HasRequired(ur => ur.User)
            //        .WithMany(r => r.UserRoles)
            //        .HasForeignKey(ur => ur.UserId);

        }

     

    }

  
}
