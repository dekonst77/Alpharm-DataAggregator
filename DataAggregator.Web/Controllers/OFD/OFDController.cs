using DataAggregator.Core.Classifier;
using DataAggregator.Core.Filter;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web.Mvc;

using DataAggregator.Domain.Model.Retail.Report;
using DataAggregator.Web.Models.Retail.Report;
using DataAggregator.Web.App_Start;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DataAggregator.Web.Controllers.OFD
{
    [Authorize(Roles = "OFD_View")]
    public class OFDController : BaseController
    {
        [HttpGet]
        public ActionResult Log()
        {
            ViewBag.log_dt = DateTime.Now.AddDays(-7);//.ToString("dd.MM.yyyy");
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ViewBag
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult Log(string dts)
        {
            using (var _ofd_context = new OFDContext(APP))
            {
                DateTime dt = DateTime.Now.AddDays(-7);
                if (!string.IsNullOrEmpty(dts))
                {
                    dt = DateTime.ParseExact(dts.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                }
                var res = _ofd_context.Logs.Where(w => w.dt >= dt).OrderBy(o => o.Id);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = res.ToList(), count = res.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
        }
        [HttpGet]
        public ActionResult Action()
        {
            
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ViewBag
            };
            return jsonNetResult;
        }
        //[HttpPost]
        //[Authorize(Roles = "OFD_Boss")]
        //public ActionResult Action(string name)
        //{
        //    using (var _ofd_context = new OFDContext(APP))
        //    {
        //        switch (name)
        //        {
        //            case "OFD_job_ftp_upload":
        //            case "OFD_AGG_daily":
        //            case "OFD_AGG_DI":
        //            case "OFD_AGG_DI_client":
        //            case "OFD_job_ftp_upload_MonthQuart":
        //            case "OFD_tovar_to_Provisor":
        //            case "OFD_tovar_to_Supplier":
        //            case "OFD_AGG_TEST":
        //            case "OFD_AGG_client":
        //            case "OFD_4SC":
        //            case "OFD_4SC_Client":
        //                DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(_ofd_context, name, Domain.Model.ControlALG.ControlALG.JobStartAction.start);
        //                break;
        //             case "Hydra_55":
        //                using (var context_Class = new DrugClassifierContext(APP))
        //                {
        //                    context_Class.StartHydra("LP", 55);
        //                }
        //                break;
        //        }
        //        JsonNetResult jsonNetResult = new JsonNetResult
        //        {
        //            Formatting = Formatting.Indented,
        //            Data = new JsonResult() { Data = "", count = 0, status = "ок", Success = true }
        //        };
        //        return jsonNetResult;
        //    }
        //}

        [HttpGet]
        public ActionResult Files()
        {
            ViewBag.files_dt = DateTime.Now.AddDays(-90);//.ToString("dd.MM.yyyy");
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ViewBag
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult Suppliers(bool withDef)
        {
            using (var _ofd_context = new OFDContext(APP))
            {
                var ret = _ofd_context.Supplier.OrderBy(o => o.value).ToList();
                if (withDef)
                {
                    ret.Add(new Domain.Model.OFD.Supplier() { Id = 0, value = "все" });
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ret.ToList(), count = ret.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
        }
        [HttpPost]
        public ActionResult Files(string dts)
        {
            using (var _ofd_context = new OFDContext(APP))
            {
                DateTime dt = DateTime.Now.AddDays(-7);
                if (!string.IsNullOrEmpty(dts))
                {
                    dt = DateTime.ParseExact(dts.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                }
                var res = _ofd_context.ofdFilenames_Views.Where(w => w.dt_load >= dt).OrderByDescending(o => o.Id);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = res.ToList(), count = res.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
        }
        public ActionResult FilesAction(string Filename,bool withFile)
        {
            using (var _ofd_context = new OFDContext(APP))
            {
                string username = User.Identity.GetUserName();
                _ofd_context.File_Delete_To_Update(Filename, username, withFile);
                return null;
            }
        }

        [HttpGet]
        public ActionResult Report()
        {
            using (var _ofd_context = new OFDContext(APP))
            {
                    var brick_3 = _ofd_context.Brick_L3_all.OrderBy(o => o.L3_label).ToList();
                    brick_3.Insert(0, new Domain.Model.OFD.Brick_L3_all() { Id = "%", L3_label = "Все" });

                    var res = _ofd_context.List.OrderByDescending(o => o.value).ToList();

                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = new JsonResult() { Data = res, Data2 = brick_3, count = res.Count(), status = "ок", Success = true }
                    };
                    return jsonNetResult;
            }
        }
        [HttpPost]
        public ActionResult Report(int Id,int supplierID,string text, DateTime period_start, DateTime period_end, string Brick_L3)
        {
            using (var _ofd_context = new OFDContext(APP))
            {

                string email = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().GetEmail(User.Identity.GetUserId());
                //System.Web.Security.Membership.GetUser().Email;

                _ofd_context.report_List_Get(Id, email, supplierID, text, period_start, period_end, Brick_L3);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = "", count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
        }

        [HttpGet]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Periods()
        {
            using (var _ofd_context = new OFDContext(APP))
            {
                var res = _ofd_context.Aggregated_Period.OrderByDescending(o => o.period);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = res.ToList(), count = res.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
        }
        [HttpGet]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult PeriodsWK()
        {
            var _ofd_context = new OFDContext(APP);
            var res = _ofd_context.Week_Periods.OrderByDescending(o => o.period_wk);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = res.ToList(), count = res.Count(), status = "ок", Success = true }
            };
            return jsonNetResult;
        }

        [HttpGet]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Periods_4SC()
        {
            var _ofd_context = new OFDContext(APP);
                ViewData["Period_4SC"] = _ofd_context.Period_4SC.OrderByDescending(o => o.period).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewData, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
        }

        [HttpPost]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Periods_save(ICollection<DataAggregator.Domain.Model.OFD.Aggregated_Period> array)
        {
            try
            {
                var _context = new OFDContext(APP);

                foreach (var item in array)
                {
                    var upd = _context.Aggregated_Period.Where(w => w.Id == item.Id).Single();

                    upd.period_type = item.period_type;
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult PeriodsWK_save(ICollection<DataAggregator.Domain.Model.OFD.Week_Periods> array)
        {
            try
            {
                var _context = new OFDContext(APP);

                foreach (var item in array)
                {
                    var upd = _context.Week_Periods.Where(w => w.Id == item.Id).Single();

                    upd.period_type = item.period_type;
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Periods_4SC_save(ICollection<DataAggregator.Domain.Model.OFD.Period_4SC> array)
        {
            try
            {
                var _context = new OFDContext(APP);

                foreach (var item in array)
                {
                    var upd = _context.Period_4SC.Where(w => w.Id == item.Id).Single();

                    upd.period_type = item.period_type;
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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


        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Agg_Init()
        {
            try
            {
                var _context = new OFDContext(APP);
                ViewData["Supplier"] = _context.Supplier.OrderBy(o => o.Id).ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Agg_search(long ClassifierId, int SupplierId, DateTime period, int BrickId)
        {
            try
            {
                var _context = new OFDContext(APP);
                period = new DateTime(period.Year, period.Month, period.Day);
                _context.Database.CommandTimeout = 0;
                ViewData["Agg"] = _context.Aggregated_All.Where(w=>w.ClassifierId==ClassifierId && w.SupplierId== SupplierId && w.period== period && w.BrickId== BrickId).ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Agg_save(
            ICollection<DataAggregator.Domain.Model.OFD.Aggregated_All> array
            )
        {
            try
            {
                var _context = new OFDContext(APP);
                if (array != null)
                    foreach (var item in array)
                    {
                        var UPD = _context.Aggregated_All.Where(w => w.Id == item.Id).Single();
                        UPD.amount = item.amount;
                        UPD.summa = item.summa;
                    }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }




        [Authorize(Roles = "OFD_Boss")]
        public ActionResult D4SS_Init()
        {
            try
            {
                var _context = new OFDContext(APP);
                ViewData["Supplier"] = _context.Supplier.OrderBy(o => o.Id).ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult D4SS_search()
        {
            try
            {
                var _context = new OFDContext(APP);
                //period = new DateTime(period.Year, period.Month, period.Day);
                ViewData["D4SS"] = _context.Data_All_4SC.Where(w => w.ClassifierId_hand==null && w.ClassifierId_korr ==null).ToList();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult D4SS_save(
            ICollection<DataAggregator.Domain.Model.OFD.Data_All_4SC> array
            )
        {
            try
            {
                var _context = new OFDContext(APP);
                if (array != null)
                    foreach (var item in array)
                    {
                        var UPD = _context.Data_All_4SC.Where(w => w.Id == item.Id).Single();
                        UPD.ClassifierId_hand = item.ClassifierId_hand;
                        UPD.SellingCountCorr = item.SellingCountCorr;
                    }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult D4SS_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql4\OFD_data\temp\4sc.xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new OFDContext(APP);
            _context.Database.CommandTimeout = 0;
            _context.Database.ExecuteSqlCommand("exec [4SC].[Data_Update_Excel]");

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
    }
    public class JsonResult
    {
        public object Data { get; set; }
        public object Data2 { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public bool Success { get; set; }
    }
}