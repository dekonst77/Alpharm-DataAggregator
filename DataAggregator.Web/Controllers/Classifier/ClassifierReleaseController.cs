using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail.SendToClassification;
using DataAggregator.Web.App_Start;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class ClassifierReleaseController : BaseController
    {

        private enum ActionType
        {
            Run,
            Stop
        }

        private DrugClassifierContext _context;

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        private async Task<string> FullUserName()
        {

            var userId = User.Identity.GetUserId();

            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            return user.FullName;

        }

        private async void JobLog(string jobName, ActionType action)
        {
            var log = new JobInfoLog();

            var actionString = String.Empty;

            switch (action)
            {
                case ActionType.Run:
                    actionString = "Запустил";
                    break;
                case ActionType.Stop:
                    actionString = "Остановил";
                    break;
            }

            log.Step = actionString + " " + await FullUserName();
            log.JobName = jobName;
            _context.JobInfoLog.Add(log);
            _context.SaveChanges();
        }

       

   


        #region Отправить данные на обработку


        [HttpPost]
        public ActionResult Initialize()
        {   
            
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ""
            };
        }

        #endregion  


    

    }
}