using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using DataAggregator.Domain.Model.Retail.CTM;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager, Ecom")]
    public class CTMController : BaseController
    {
        private readonly RetailContext _context;

        public CTMController()
        {
            _context = new RetailContext(APP);
        }

        ~CTMController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult LoadCTM(int year, int month)
        {
            try
            {
                var result = _context.LoadCTMView(year, month);
                ViewData["CTM"] = result;
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }
        }

        [HttpPost]
        public ActionResult Edit(DateTime period, Domain.Model.Retail.CTM.CTMView record, string fieldname)
        {
            try
            {
                var result = _context.LoadCTMRecord(period, record, fieldname);
                ViewData["CTMRecord"] = result.ToArray();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    msg += e.Message;
                }
                return BadRequest(msg);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Network_FromExcelAsync(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            string filename = @"\\s-sql3\FileUpload\Network_" + User.Identity.GetUserId() + ".xlsx";
            try
            {
                var file = uploads.First();
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);

                file.SaveAs(filename);

                var _context = new RetailContext(APP);
                await _context.Network_FromExcelAsync(User.Identity.GetUserId());

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null }
                };

                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            finally
            {
                System.IO.File.Delete(filename);
            }
        }

        [HttpPost]
        public ActionResult ClearComment(int Id)
        {
            var ctmRecord = new CTMView { Id = Id };
            _context.CTMView.Attach(ctmRecord);
            ctmRecord.Comment = String.Empty;
            ctmRecord.Date_modifired = DateTime.Now;

            _context.SaveChanges();

            return new EmptyResult();
        }

        [HttpPost]
        public async Task<ActionResult> GetAllNetworks(int year, int month)
        {
            try
            {
                var result = await _context.GetAllNetworksAsync(year, month);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };

                return jsonNetResult;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    msg += e.Message;
                }
                return BadRequest(msg);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GS_periodsAsync()
        {
            using (var gs_context = new GSContext(APP))
            {
                List<DateTime> PeriodList = await gs_context.GS_Period.Select(s => s.period).Distinct().OrderByDescending(o => o).Take(6).ToListAsync();

                if ((PeriodList == null) | (PeriodList.Count == 0))
                {
                    return NotFound();
                }

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = PeriodList.Select(s => s.ToString("yyyy-MM"))
                };
            }
        }

    }

}
