using DataAggregator.Web.App_Start;
using DataAggregator.Web.Models.Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers
{
    public sealed class UserServiceController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetUser()
        {
            UserServiceModel U = null;
            if (!User.Identity.IsAuthenticated)
            {
                U = new UserServiceModel(User, null);
            }
            else
            {
                U = new UserServiceModel(User, HttpContext);
                U.UserId = aspUser.UserId;
                //U.UserId = aspUser.User_Email;
                U.Fullname = aspUser.User_FullName;
            }
            
            return Json(U, JsonRequestBehavior.AllowGet);
        }
    }
}