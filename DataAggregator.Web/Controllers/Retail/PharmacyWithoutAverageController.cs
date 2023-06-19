using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss")]
    public class PharmacyWithoutAverageController : BaseController
    {
        private RetailCalculationContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new RetailCalculationContext();
        }

        ~PharmacyWithoutAverageController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetList(int year, int month, bool isGenerated = false)
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new { 
                    In = _context.TargetPharmacyWithoutAverageIn.Where(x => x.Year == year && x.Month == month && x.IsGenerated == isGenerated).ToList(),
                    Out = _context.TargetPharmacyWithoutAverageOut.Where(x => x.Year == year && x.Month == month && x.IsGenerated == isGenerated).ToList(),
                }
            };
        }

        public ActionResult UploadFromExcel(int month, int year, HttpPostedFileBase file)
        {
            try
            {
                using (var _context = new RetailCalculationContext())
                {
                    string filename = @"\\s-sql2\Upload\TargetPharmacyWithoutAverage_" + User.Identity.GetUserId() + ".xlsx";

                    if (System.IO.File.Exists(filename))
                        System.IO.File.Delete(filename);

                    file.SaveAs(filename);

                    _context.ImportTargetPharmacyWithoutAverage_from_Excel(month, year, filename);
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonNetResult() { Data = null}
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}