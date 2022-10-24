using DataAggregator.Web.App_Start;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataAggregator.Web
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationUser : IdentityUser<string,IdentityUserLogin,ApplicationUserRole,IdentityUserClaim>
    {
        public DateTime? LockDate { get; set; }

        private string _surname;
        private string _firstName;
        private string _middleName;

        public string Surname
        {
            get { return _surname ?? ""; }
            set { _surname = value; }
        }

        public string FirstName
        {
            get { return _firstName ?? ""; }
            set { _firstName = value; }
        }

        public string MiddleName
        {
            get { return _middleName ?? ""; }
            set { _middleName = value; }
        }

        /// <summary>
        /// Идентификатор подразделения
        /// </summary>
        public long? DepartmentId { get; set; }

        //public int UserId { get; set; }
        /// <summary>
        /// Множественная аутентификация
        /// </summary>
        public bool MultipleAuthentication { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        [NotMapped]
        public string FullName 
        {
            get { return Surname + " " + FirstName + " " + MiddleName; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public string Category { get; set; }
        public string Description { get; set; }
        //public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        // public virtual ApplicationUser User { get; set; }
         public virtual ApplicationRole Role { get; set; }
    }
}