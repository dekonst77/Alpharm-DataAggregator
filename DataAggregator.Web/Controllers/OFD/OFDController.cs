#define DebugOnProductDB
using DataAggregator.Domain.DAL;
using DataAggregator.Web.App_Start;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult FilesAction(string Filename, bool withFile)
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
        public ActionResult Report(int Id, int supplierID, string text, DateTime period_start, DateTime period_end, string Brick_L3)
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
        public ActionResult Agg_search(int SupplierId, DateTime periodStart, DateTime periodEnd, int BrickId, long? ClassifierId = null)
        {
            try
            {
                using (var _context = new OFDContext(APP))
                {
                    _context.Database.CommandTimeout = 0;

                    ViewData["Agg"] = _context.GetAggsearch_Result(SupplierId, periodStart, periodEnd, BrickId, ClassifierId).ToList();

                    var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = Data
                    };
                    return jsonNetResult;
                }
                /*
                using (var txn = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions {IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    using (var _context = new OFDContext(APP))
                    {
                        period = new DateTime(period.Year, period.Month, period.Day);
                        _context.Database.CommandTimeout = 0;
                        ViewData["Agg"] = _context.Aggregated_All.Where(w => w.ClassifierId == ClassifierId && w.SupplierId == SupplierId && w.period == period && w.BrickId == BrickId).ToList();
                        var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                        JsonNetResult jsonNetResult = new JsonNetResult
                        {
                            Formatting = Formatting.Indented,
                            Data = Data
                        };
                        return jsonNetResult;
                    }
                }
                */
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

        [HttpPost]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Agg_remove(
            List<long> aggToDelete
            )
        {
            try
            {
                var _context = new OFDContext(APP);
                if (aggToDelete != null)
                {
                    _context.Aggregated_All.RemoveRange(_context.Aggregated_All.Where(agg => aggToDelete.Contains(agg.Id)));
                    _context.SaveChanges();
                }
                
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
                ViewData["D4SS"] = _context.Data_All_4SC.Where(w => w.ClassifierId_hand == null && w.ClassifierId_korr == null).ToList();
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

        [HttpPost]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult Network_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string fileName = file.FileName;

            ///<remarks>тестовая БД: [alph-r01-s-db02].[OFD_data]</remarks>
#if DEBUG && !DebugOnProductDB
            string ServerPath = @"S:\Report\";
#else
            string ServerPath = @"E:\OFD_data\temp\";
#endif

            string FullFileName = ServerPath + fileName;

            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["OFDContext"].ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "dbo.usp_CreateFile";
                    command.Parameters.Add("@File", SqlDbType.VarChar).Value = FullFileName;
                    command.Parameters.Add("@Content", SqlDbType.VarBinary).Value = data;
                    command.ExecuteNonQuery();
                }
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "dbo.Aggregated_All_EditByExcel";
                    command.Parameters.Add("@File", SqlDbType.VarChar).Value = FullFileName;
                    command.Parameters.Add("@updData", SqlDbType.Bit).Value = 0;
                    command.ExecuteNonQuery();
                }
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null }
            };

            return jsonNetResult;
        }


        [Authorize(Roles = "OFD_Boss")]
        public ActionResult D4SC_Agreement_Init()
        {
            try
            {
                var _context = new OFDContext(APP);
                ViewData["Supplier"] = _context.Supplier.OrderBy(o => o.Id).ToList();

                ViewData["NetworkNames"] = _context.Agreement_All
                    .GroupBy(x => new { x.SupplierId } )
                    .Select(x => new {
                        SupplierId = x.Key.SupplierId,
                        Networks = x.Select(n => n.NetworkName).Distinct()
                    }).ToList();

                ViewData["EntityINNs"] = _context.Agreement_All
                    .GroupBy(x => new { x.SupplierId, x.NetworkName })
                    .Select(x => new {
                        SupplierId = x.Key.SupplierId,
                        NetworkName = x.Key.NetworkName,
                        INNs = x.Select(n => n.EntityINN).Distinct()
                    }).ToList();

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
        public ActionResult D4SC_Agreement_search(int? SupplierId = null, DateTime? periodStart = null, DateTime? periodEnd = null, string NetworkName = null, string[] EntityINNs = null, bool isCurrent = false)
        {
            try
            {
                var _context = new OFDContext(APP);
                var innList = EntityINNs != null && EntityINNs.Length > 0 ? EntityINNs.ToList() : null;
                var today = DateTime.Today;

                var data = _context.Agreement_All
                    .AsNoTracking()
                    .Where(x => (SupplierId == null || x.SupplierId == SupplierId)
                        && (periodStart == null || x.Date_begin >= periodStart)
                        && (periodEnd == null || x.Date_end <= periodEnd)
                        && (NetworkName == null || x.NetworkName == NetworkName)
                        && (isCurrent == false || x.Date_end >= today));

                if (innList != null)
                    data = data.Where(x => innList.Contains(x.EntityINN));

                ViewData["D4SC_Agreement"] = data.OrderByDescending(x => x.AgreementId).ToList();

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
        public ActionResult D4SC_Agreement_Classifiers(int AgreementId)
        {
            try
            {
                var _context = new OFDContext(APP);
                ViewData["Classifiers"] = _context.AgreementClassifiers
                    .AsNoTracking()
                    .Where(x => x.AgreementId == AgreementId).ToArray();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data, status = "ок", Success = true }
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
        public ActionResult ImportAgreements_from_Excel(IEnumerable<HttpPostedFileBase> uploads, bool force = false)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    return null;

                using (var _context = new OFDContext(APP))
                {

                    var file = uploads.First();
                    string filename = @"\\s-sql2\Upload\Agreements_" + User.Identity.GetUserId() + ".xlsx";

                    if (System.IO.File.Exists(filename))
                        System.IO.File.Delete(filename);

                    file.SaveAs(filename);

                    _context.ImportAgreements_from_Excel(filename, force);
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult D4SC_Agreement_save(int[] array, DateTime dateBegin, DateTime dateEnd)
        {
            try
            {
                var _context = new OFDContext(APP);
                if (array != null)
                {
                    DateTime beforeDatEnd = dateBegin.AddDays(-1);

                    foreach (var item in array)
                    {
                        var UPD = _context.Agreement_All.Where(x => x.AgreementId == item).FirstOrDefault();
                        if (UPD != null)
                        {
                            if (UPD.Date_end >= beforeDatEnd)
                                UPD.Date_end = beforeDatEnd;

                            var ADD = new Domain.Model.OFD.Agreement
                            {
                                SupplierId = UPD.SupplierId,
                                Name = UPD.Name,
                                NetworkName = UPD.NetworkName,
                                EntityINN = UPD.EntityINN,
                                OwnerAgrId = UPD.OwnerAgrId,
                                OwnerAgr = UPD.OwnerAgr,
                                Date_begin = dateBegin,
                                Date_end = dateEnd
                            };
                            _context.Agreement_All.Add(ADD);

                            var Classifiers = _context.AgreementClassifiers.Where(x => x.AgreementId == item).ToList();
                            if (Classifiers != null && Classifiers.Any())
                            {
                                ADD.Classifiers = new List<Domain.Model.OFD.Classifier>();
                                ADD.Classifiers.AddRange(Classifiers.Select(x => new Domain.Model.OFD.Classifier { AgreementId = 0, ClassifierId = x.ClassifierId }).ToList());
                            }
                        }
                    }

                    _context.SaveChanges();
                }

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
        [Authorize(Roles = "OFD_Boss")]
        public ActionResult GS_ToOFD_search(string filter, string pharmacyId, string inn)
        {
            try
            {
                var _context = new GSContext(APP);
                int result;

                int[] pharmacyArr = !string.IsNullOrEmpty(pharmacyId) ? pharmacyId.Split(',').Select(x => Int32.TryParse(x, out result) ? result : -1).Distinct().ToArray() : new int[] { };
                string[] innArr = !string.IsNullOrEmpty(inn) ? inn.Split(',') : new string[] { };

                var data = _context.GS
                    .Where(x => (filter == null || filter != null && x.Address.Contains(filter))
                        && (!pharmacyArr.Any() || pharmacyArr.Contains(x.PharmacyId ?? 0))
                        && (!innArr.Any() || innArr.Contains(x.EntityINN)));

                ViewData["GS_ToOFD"] = data.OrderByDescending(x => x.Id).ToList();

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
        public ActionResult GS_ToOFD_save(int[] array, string comment)
        {
            try
            {
                if (string.IsNullOrEmpty(comment))
                    throw new Exception("Не заполнен комментарий");

                if (array != null)
                {
                    var _context = new GSContext(APP);
                    foreach (var item in array)
                    {
                        var UPD = _context.GS.FirstOrDefault(x => x.Id == item);
                        if (UPD != null)
                        {
                            UPD.ToOFD = false;
                            UPD.ToOFDComment = comment;
                        }
                    }
                    _context.SaveChanges();
                }

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

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