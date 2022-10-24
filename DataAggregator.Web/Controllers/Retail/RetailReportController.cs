using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail.Report;
using DataAggregator.Web.Models.Retail.Report;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using DataAggregator.Web.App_Start;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss")]
    public class RetailReportController : BaseController
    {
        private readonly RetailContext _context;

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

        public RetailReportController()
        {           
            _context = new RetailContext();
        }

        ~RetailReportController()
        {
            _context.Dispose();
        }

        // GET: RetailReport
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetReports()
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.Report.OrderByDescending(r => r.Id).Take(100).ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult> AddReports(List<ReportLauncherModel> models)
        {

            List<ApplicationUser> users = await UserManager.Users.OrderBy(u => u.Surname).ToListAsync();
            ApplicationUser user = users.First(u => u.Id == User.Identity.GetUserId());
            //ApplicationUser user = UserManager.Users.First(u => u.Id == User.Identity.GetUserId());

            foreach (ReportLauncherModel model in models)
            {
                ReportLauncher launcher = ModelMapper.Mapper.Map<ReportLauncher>(model);
                launcher.UserId = new Guid(user.Id);
                launcher.StatusId = 1;

                if(model.SendSelf)
                {
                    launcher.Email = user.Email;
                }

                _context.ReportLauncher.Add(launcher);
            }

            _context.SaveChanges();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented
           
            };
        }

        [HttpPost]
        public ActionResult StopReports(List<long> reportId)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {

                var reports = _context.ReportLauncher.Where(s => reportId.Contains(s.Id)).ToList();

                reports.ForEach((item) =>
                {
                    if (item.StatusId == 1)
                    {
                        item.StatusId = 4;
                        item.UserId = new Guid(User.Identity.GetUserId());
                        item.DateEnd = DateTime.Now;
                    }
                });

                _context.SaveChanges();

                transaction.Commit();               

            }


            return new JsonNetResult
            {
                Formatting = Formatting.Indented

            };
        }



        [HttpPost]
        public ActionResult GetReportLauncher()
        {

            var launcher = _context.ReportLauncher.ToList();

         
            List<ReportLauncherModel> models = launcher.Select(ReportLauncherModel.Create).ToList();

          
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = models
            };
        }
    }
}