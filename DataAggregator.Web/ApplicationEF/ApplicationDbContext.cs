using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DataAggregator.Web
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, ApplicationUserRole, IdentityUserClaim>
    {
        public ApplicationDbContext()
            : base("DataAggregatorContext")
        {
            
        }

        public DbSet<Department> Departments { get; set; }
        //public DbSet<AspNetRoles> AspNetRoles { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}