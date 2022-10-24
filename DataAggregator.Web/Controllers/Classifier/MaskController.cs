using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{
    public class MaskController : BaseController
    {
        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Load()
        {

            _context.UpdateMask();
            var result = _context.MaskView.ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult ChangeBlock(int Id)
        {
            dynamic result = new ExpandoObject();
            var userGuid = new Guid(User.Identity.GetUserId());

            var mask = _context.Mask.SingleOrDefault(m => m.Id == Id);

            if (mask == null)
            {
                result.Message = "Такое Mask еще не существует";
                result.Success = false;


                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
            }
            else
            {
                mask.Use = !mask.Use;
                mask.UserId = userGuid;
                mask.DateUpdate = DateTime.Now;


                _context.SaveChanges();
            }

            result.Message = "Такое Mask еще не существует";
            result.Success = true;
            result.DateUpdate = mask.DateUpdate;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

    }
}