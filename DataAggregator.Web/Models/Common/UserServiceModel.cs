using DataAggregator.Web.App_Start;
using DataAggregator.Web.Models;
using DataAggregator.Web.Models.Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Models.Common
{
    public class UserServiceModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Fullname { get; set; }
        public string[] Roles { get; set; }
        public bool IsAuthenticated { get; set; }        
        public int UserId { get; set; }

        public UserServiceModel(System.Security.Principal.IPrincipal user, HttpContextBase httpc)
        {
            if (!user.Identity.IsAuthenticated)
            {
                Roles = new string[0];
            }
            else
            {
                var userManager = httpc.GetOwinContext().GetUserManager<ApplicationUserManager>();

                Name = user.Identity.Name;
                Roles = userManager.GetRoles(user.Identity.GetUserId()).ToArray();
                IsAuthenticated = user.Identity.IsAuthenticated;
                Id = user.Identity.GetUserId();
                Fullname = user.Identity.GetUserName();
            }            
        }
    }
}