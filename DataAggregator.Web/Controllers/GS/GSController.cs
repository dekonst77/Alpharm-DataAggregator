using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GS;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GS
{
    [Authorize(Roles = "GS_View")]
    public class GSController : BaseController
    {/*GS_Licenses
GS_Reestr
GS_View
GS_Brick*/
        //private readonly GSContext _context;

        public GSController()
        {


        }
        // GET: GS
        public ActionResult GS_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                var p = _context.GS_Period.Select(s => s.period).Distinct().OrderByDescending(o => o).ToList().Select(s => s.ToString("yyyy-MM"));
                var spr_FormatLayout = _context.spr_FormatLayout.Select(s => new { code = s.Id, status = s.Value }).Distinct().OrderBy(o => o.status).ToList();
                var spr_PharmacySellingPlaceType = _context.spr_PharmacySellingPlaceType.Select(s => new { code = s.Id, status = s.Value }).Distinct().OrderBy(o => o.status).ToList();
                var spr_WorkFormat = _context.spr_WorkFormat.Select(s => new { code = s.Id, status = s.Value }).Distinct().OrderBy(o => o.status).ToList();
                var spr_PointCategory = _context.PointCategory.Select(s => new { code = s.Id, status = s.Name }).Distinct().OrderBy(o => o.status).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult()
                    {
                        Data = new GS_init_class()
                        {
                            periods = p,
                            spr_FormatLayout = spr_FormatLayout,
                            spr_PointCategory = spr_PointCategory,
                            spr_PharmacySellingPlaceType = spr_PharmacySellingPlaceType,
                            spr_WorkFormat = spr_WorkFormat
                        },
                        count = 0,
                        status = "ок",
                        Success = true
                    }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult GS_search(FilterReestr filter, DateTime currentperiod)
        {
            if (filter.BrickId == null)
                filter.BrickId = "";
            currentperiod = currentperiod.AddDays(14);
            if (filter.IDS == null) filter.IDS = "";
            if (filter.PHids == null) filter.PHids = "";
            if (filter.common == null) filter.common = "";
            if (filter.adress == null) filter.adress = "";
            if (filter.BrickId == null) filter.BrickId = "";
            if (filter.NetworkName == null) filter.NetworkName = "";
            if (filter.PharmacyBrand == null) filter.PharmacyBrand = "";
            if (filter.OperationMode == null) filter.OperationMode = "";
            if (filter.dt == null) filter.dt = new DateTime(1900, 1, 1);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.Database.SqlQuery<Domain.Model.GS.GS_View_SP>("dbo.GS_View_SP @filter,@IDS,@PHids,@OperationMode,@adress,@period,@BrickId,@NetworkName,@PharmacyBrand,@isNotChecked,@isNew,@isCloseOFD,@isCloseAlphaBit,@isDoubleA,@isLicExists,@isCall,@isDateAddLic,@dt,@BrickError",
                    new System.Data.SqlClient.SqlParameter("@filter", filter.common),
                    new System.Data.SqlClient.SqlParameter("@IDS", filter.IDS),
                    new System.Data.SqlClient.SqlParameter("@PHids", filter.PHids),
                    new System.Data.SqlClient.SqlParameter("@OperationMode", filter.OperationMode),
                    new System.Data.SqlClient.SqlParameter("@adress", filter.adress),
                    new System.Data.SqlClient.SqlParameter("@BrickId", filter.BrickId),
                    new System.Data.SqlClient.SqlParameter("@NetworkName", filter.NetworkName),
                    new System.Data.SqlClient.SqlParameter("@PharmacyBrand", filter.PharmacyBrand),
                    new System.Data.SqlClient.SqlParameter("@isNotChecked", filter.isNotChecked),
                    new System.Data.SqlClient.SqlParameter("@isNew", filter.isNew),
                    new System.Data.SqlClient.SqlParameter("@isCloseOFD", filter.isCloseOFD),
                    new System.Data.SqlClient.SqlParameter("@isCloseAlphaBit", filter.isCloseAlphaBit),
                    new System.Data.SqlClient.SqlParameter("@isDoubleA", filter.isDoubleA),
                    new System.Data.SqlClient.SqlParameter("@isLicExists", filter.isLicExists),
                    new System.Data.SqlClient.SqlParameter("@isCall", filter.isCall),
                    new System.Data.SqlClient.SqlParameter("@isDateAddLic", filter.isDateAddLic),
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@dt", SqlDbType = System.Data.SqlDbType.Date, Value = filter.dt },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = currentperiod },
                    new System.Data.SqlClient.SqlParameter("@BrickError", filter.BrickError)

                ).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult PharmacyId_new(int GSId)
        {

            try
            {
                var _context = new GSContext(APP);
                var gs = _context.GS.Where(w => w.Id == GSId).FirstOrDefault();
                var Ph = _context.Pharmacy.Add(new Domain.Model.GS.Pharmacy()
                {
                    date_add = DateTime.Now,
                    GSId_first = GSId,
                    Address_region = gs.Address_region,
                    Address_city = gs.Address_city,
                    Address_index = gs.Address_index,
                    Address_street = gs.Address_street,
                    Address_comment = gs.Address_comment,
                    Address_float = gs.Address_float,
                    Address_room = gs.Address_room,
                    Address_room_area = gs.Address_room_area
                });
                _context.SaveChanges();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Ph, count = 1, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult GS_Merge(List<int> GSIds)
        {
            try
            {
                var _context = new GSContext(APP);
                int GS_min = GSIds.Min();
                GSIds.Remove(GS_min);
                
                _context.GS_Merge(GS_min, GSIds);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = GSIds, count = 1, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult GS_delete(int GSId)
        {
            try
            {
                var _context = new GSContext(APP);
                
                _context.GS_delete(GSId);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = GSId, count = 1, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult GS_save(ICollection<DataAggregator.Domain.Model.GS.GS_View_SP> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    item.NotNull();
                    DataAggregator.Domain.Model.GS.GS gs = null;
                    if (item.Id == 0 && item.Address == null)
                    {
                        continue;
                    }
                    if (item.Id == 0)
                    {//Считать за новых
                        gs = new Domain.Model.GS.GS
                        {
                            Address = item.Address,
                            LeaseExpirationDate = new DateTime(1900, 1, 1),
                            LeaseForm = "",
                            MonthlyTurnover = 0,
                            AverageReceipt = 0,
                            CashOfficeCount = 0,
                            EntityType = "",
                            SKU_Count = 0
                        };
                    }
                    else
                    {
                        gs = _context.GS.Where(w => w.Id == item.Id).FirstOrDefault();
                        if (gs == null)
                            continue;
                    }
                    //--скрыть gs.ABC_Category = item.ABC_Category;
                    //gs.adress = item.adress;Исходное поле не обновлять!
                    //--скрытьgs.AverageReceipt = item.AverageReceipt;
                    gs.BricksId = item.BricksId;
                    //--скрытьgs.CashOfficeCount = item.CashOfficeCount;
                    gs.ContactPersonFullname = item.ContactPersonFullname;
                    gs.EntityINN = item.EntityINN;
                    gs.EntityName = item.EntityName;
                    //--скрытьgs.EntityType = item.EntityType;
                    gs.FormatLayout = item.FormatLayout;

                    gs.Address_street = item.Address_street;
                    gs.Address_city = item.Address_city;
                    gs.Address_region = item.Address_region;
                    gs.Address_index = item.Address_index;
                    gs.Address_float = item.Address_float;
                    gs.Address_room = item.Address_room;
                    gs.Address_room_area = item.Address_room_area;
                    gs.Address_koor = item.Address_koor;
                    gs.Address_comment = item.Address_comment;
                    gs.Comment = item.Comment;

                    //--скрыть gs.LeaseExpirationDate = item.LeaseExpirationDate;
                    //--скрытьgs.LeaseForm = item.LeaseForm;
                    //--скрытьgs.MonthlyTurnover = item.MonthlyTurnover;
                    gs.OperationMode = item.OperationMode;
                    gs.PharmacyBrand = item.PharmacyBrand;
                    gs.PharmacyId = item.PharmacyId;
                    gs.PharmacyNumber = item.PharmacyNumber;
                    gs.PharmacySellingPlaceType = item.PharmacySellingPlaceType;
                    gs.Phone = item.Phone;
                    gs.PKU = item.PKU;
                    gs.ECom = item.ECom;
                    //--скрытьgs.SKU_Count = item.SKU_Count;
                    if (gs.UserControl_Name != item.UserControl_Name)
                    {
                        gs.UserControl_Name = item.UserControl_Name;
                        gs.UserControl_dt = DateTime.Now;
                    }
                    gs.Website = item.Website;
                    gs.WorkFormat = item.WorkFormat;


                    if (item.Id == 0)
                    {
                        _context.SaveChanges();
                        _context.GS.Add(gs);
                        _context.SaveChanges();
                        item.Id = gs.Id;
                        _context.GS_Period_AddAll();

                    }

                    var period = _context.GS_Period.Where(w => w.GSId == gs.Id && w.period == item.period).Single();
                    if (period.NetworkName != item.NetworkName /*|| period.Summa != item.Summa*/ || period.isExists != item.isExists)
                    {
                        period.NetworkName = item.NetworkName;
                        //--скрытьperiod.Summa = item.Summa;
                        period.isExists = item.isExists;
                    }

                    DateTime prev_period1 = item.period.AddMonths(-1);
                    var period_p1 = _context.GS_Period.Where(w => w.GSId == gs.Id && w.period == prev_period1).Single();
                    if (period_p1.NetworkName != item.NetworkName_p1 /*|| period_p1.Summa != item.Summa_p1*/ || period_p1.isExists != item.isExists_p1)
                    {
                        period_p1.NetworkName = item.NetworkName_p1;
                        //--скрытьperiod_p1.Summa = item.Summa_p1;
                        period_p1.isExists = item.isExists_p1;
                    }
                    DateTime prev_period2 = item.period.AddMonths(-2);
                    var period_p2 = _context.GS_Period.Where(w => w.GSId == gs.Id && w.period == prev_period2).Single();
                    if (period_p2.NetworkName != item.NetworkName_p2 /*|| period_p2.Summa != item.Summa_p2*/ || period_p2.isExists != item.isExists_p2)
                    {
                        period_p2.NetworkName = item.NetworkName_p2;
                        //--скрытьperiod_p2.Summa = item.Summa_p2;
                        period_p2.isExists = item.isExists_p2;
                    }
                    DateTime prev_period3 = item.period.AddMonths(-3);
                    var period_p3 = _context.GS_Period.Where(w => w.GSId == gs.Id && w.period == prev_period3).Single();
                    if (period_p3.NetworkName != item.NetworkName_p3 /*|| period_p3.Summa != item.Summa_p3*/ || period_p3.isExists != item.isExists_p3)
                    {
                        period_p3.NetworkName = item.NetworkName_p3;
                        //--скрытьperiod_p3.Summa = item.Summa_p3;
                        period_p3.isExists = item.isExists_p3;
                    }
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
        public ActionResult GS_periods(int Id)
        {
            try
            {
                var _context = new GSContext(APP);
                var result = _context.GS_Period_Lic_View.Where(w => w.GSId == Id).OrderByDescending(o => o.period).ToList();//.Take(top);
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        [Authorize(Roles = "GS_Reestr")]
        public ActionResult GS_periods_Save(ICollection<DataAggregator.Domain.Model.GS.GS_Period> array)
        {
            try
            {
                var _context = new GSContext(APP);
                foreach (var item in array)
                {
                    if (item.NetworkName == null)
                        item.NetworkName = "";
                    var upd = _context.GS_Period.Where(w => w.Id == item.Id).Single();
                    upd.isExists = item.isExists;
                    upd.NetworkName = item.NetworkName;
                    upd.Summa = item.Summa;
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
                return BadRequest(e);
            }
        }
        [HttpPost]
        [Authorize(Roles = "GS_Reestr")]
        public ActionResult GS_Clone(int GSId, DateTime currentperiod)
        {
            //Добавление клона
            var _context = new GSContext(APP);
            var R_gs = _context.GS.Where(w => w.Id == GSId).Single();
            var R_per = _context.GS_Period.Where(w => w.GSId == GSId).ToList();

            DataAggregator.Domain.Model.GS.GS clone_GS = new Domain.Model.GS.GS
            {
                Comment = "сдублировано с " + GSId.ToString(),
                Address_index = R_gs.Address_index,
                Address_city = R_gs.Address_city,
                Address_street = R_gs.Address_street,
                Address_region = R_gs.Address_region,
                Address_koor = R_gs.Address_koor,
                BricksId = R_gs.BricksId
            };
            _context.GS.Add(clone_GS);
            _context.SaveChanges();

            foreach (var el in R_per)
            {
                _context.GS_Period.Add(new Domain.Model.GS.GS_Period() { GSId = clone_GS.Id, NetworkName = el.NetworkName, period = el.period });
            }
            _context.SaveChanges();

            return GS_search(new FilterReestr() { IDS = clone_GS.Id.ToString(), isCall = false, isCloseOFD = false, isCloseAlphaBit = false, isDateAddLic = false, isDoubleA = false, isLicExists = false, isNew = false, isNotChecked = false, BrickError = false }, currentperiod); ;
        }
        [HttpPost]
        public ActionResult Calls_show(int Id)
        {
            try
            {
                var _context = new GSContext(APP);
                var result = _context.Calls.Where(w => w.GSId == Id).OrderByDescending(o => o.Creator_Date).ToList();//.Take(top);
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        [Authorize(Roles = "GS_Reestr")]
        public ActionResult Calls_Save(ICollection<DataAggregator.Domain.Model.GS.Calls> array)
        {
            try
            {
                var _context = new GSContext(APP);
                foreach (var item in array)
                {
                    var upd = _context.Calls.Where(w => w.Id == item.Id).Single();
                    if (upd.Result_text != item.Result_text && upd.IsOpen == true)
                    {
                        upd.Result_text = item.Result_text;
                        upd.Result_Date = DateTime.Now;
                        upd.Result_User = User.Identity.GetUserName();
                        upd.IsOpen = false;
                    }
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
                return BadRequest(e);
            }
        }
        /*    public System.Data.DataTable ToDataTable<T>()
            {
                System.Data.DataTable dataTable = new System.Data.DataTable(typeof(T).Name);


                //Get all the properties
                System.Reflection.PropertyInfo[] Props = typeof(T).GetProperties();// (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                //var displayName = typeof(T).GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true);
                foreach (System.Reflection.PropertyInfo prop in Props)
                {
                    MemberInfo property = typeof(T).GetProperty("Bricks_FederalDistrict");
                    var dd = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                    if (dd != null)
                    {
                        var name = dd.Name;
                    }
                }
                return null;
            }*/

        [HttpPost]
        public FileResult GS_ToExcel(ICollection<int> ids, DateTime currentperiod)
        {
            /*
            ToDataTable<DataAggregator.Domain.Model.GS.GS>();
            ToDataTable<DataAggregator.Domain.Model.GS.GS_View_SP>();*/
            var _context = new GSContext(APP);
            string IDS = "";
            if (ids != null)
            {
                foreach (var item in ids)
                {
                    if (IDS != "") IDS += ",";
                    IDS += Convert.ToString(item);
                }
            }
            if (IDS == "")
            {
                IDS = "%";
            }
            _context.Database.CommandTimeout = 0;
            var ret = _context.Database.SqlQuery<Domain.Model.GS.GS_View_SP>("dbo.GS_View_SP @filter,@IDS,@PHids,@OperationMode,@adress,@period,@BrickId,@NetworkName,@PharmacyBrand,@isNotChecked,@isNew,@isCloseOFD,@isCloseAlphaBit,@isDoubleA,@isLicExists,@isCall,@isDateAddLic,@dt,@BrickError",
                new System.Data.SqlClient.SqlParameter("@filter", ""),
                new System.Data.SqlClient.SqlParameter("@IDS", IDS),
                new System.Data.SqlClient.SqlParameter("@PHids", ""),
                new System.Data.SqlClient.SqlParameter("@OperationMode", ""),
                new System.Data.SqlClient.SqlParameter("@adress", ""),
                new System.Data.SqlClient.SqlParameter("@BrickId", ""),
                 new System.Data.SqlClient.SqlParameter("@NetworkName", ""),
                 new System.Data.SqlClient.SqlParameter("@PharmacyBrand", ""),
                new System.Data.SqlClient.SqlParameter("@isNotChecked", false),
                new System.Data.SqlClient.SqlParameter("@isNew", false),
                new System.Data.SqlClient.SqlParameter("@isCloseOFD", false),
                new System.Data.SqlClient.SqlParameter("@isCloseAlphaBit", false),
                new System.Data.SqlClient.SqlParameter("@isDoubleA", false),
                new System.Data.SqlClient.SqlParameter("@isLicExists", false),
                new System.Data.SqlClient.SqlParameter("@isCall", false),
                new System.Data.SqlClient.SqlParameter("@isDateAddLic", false),
                new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = currentperiod },
                new System.Data.SqlClient.SqlParameter { ParameterName = "@dt", SqlDbType = System.Data.SqlDbType.Date, Value = DBNull.Value },
                new System.Data.SqlClient.SqlParameter("@BrickError", false)
                ).ToList();


            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("ГС", 1, 1, ret, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "Шаблон.xlsx");
        }
        [HttpPost]
        public ActionResult GS_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\ГС_up.xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.GS_from_Excel(@"S:\Upload\ГС_up.xlsx");
            /*Excel.Excel excel = new Excel.Excel();
            excel.Open(file.InputStream);
            */
            //var row_U = excel.ToList<Domain.Model.GS.GS_View_SP>("ГС", 1, 2, () => new Domain.Model.GS.GS_View_SP());

            //return GS_save(row_U);
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [HttpPost]
        [Authorize(Roles = "GS_Reestr")]
        public ActionResult GS_restore_From_changelog(string GSIds)
        {
            try
            {
                var _context = new GSContext(APP);
                _context.GS_restore_From_changelog(GSIds);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = GSIds, count = 1, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Authorize(Roles = "GS_Reestr")]
        public ActionResult GS_SetEmpty()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.GS_PharmacyIdSet();
                _context.GS_BrickIdSet();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 1, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        public ActionResult Licenses()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = "", count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult Licenses_search(FilterLicenses filter)
        {
            try
            {
                JsonResult Data = new JsonResult();
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                if (filter.withAdress)//показать с адресами
                {
                    var result = _context.licenses_ViewAll.Select(s => s);
                    if (filter.date != null)
                    {
                        result = result.Where(w => w.date >= filter.date);
                    }
                    if (!string.IsNullOrEmpty(filter.common))
                    {
                        result = result.Where(w =>
                            w.inn.Contains(filter.common)
                            || w.name.Contains(filter.common)
                            || w.number.Contains(filter.common)
                            );
                    }
                    if (!string.IsNullOrEmpty(filter.adress))
                    {
                        result = result.Where(w =>
                            w.address.Contains(filter.adress)
                            ||
                            w.address_point.Contains(filter.adress)
                            );
                    }
                    if (!string.IsNullOrEmpty(filter.activity_type))
                    {
                        result = result.Where(w => w.activity_type.Contains(filter.activity_type));
                    }
                    if (!string.IsNullOrEmpty(filter.works))
                    {
                        result = result.Where(w => w.works.Contains(filter.works));
                    }

                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true };
                }
                else//показать только лицензии
                {
                    var result = _context.licenses.Select(s => s);
                    if (filter.date != null)
                    {
                        result = result.Where(w => w.date >= filter.date);
                    }
                    if (!string.IsNullOrEmpty(filter.common))
                    {
                        result = result.Where(w =>
                            w.inn.Contains(filter.common)
                            || w.name.Contains(filter.common)
                            || w.number.Contains(filter.common)
                            );
                    }
                    if (!string.IsNullOrEmpty(filter.adress))
                    {
                        result = result.Where(w =>
                            w.address.Contains(filter.common)
                            );
                    }
                    if (!string.IsNullOrEmpty(filter.activity_type))
                    {
                        result = result.Where(w => w.activity_type.Contains(filter.activity_type));
                    }
                    if (!string.IsNullOrEmpty(filter.works))
                    {
                        var LA = _context.licenses_adress.Where(w1 => w1.works.Contains(filter.works)).Select(s => s.licensesId);
                        result = result.Where(w => LA.Contains(w.Id));
                    }
                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true };
                }

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
        public ActionResult Bricks_Init()
        {
            var _context = new GSContext(APP);
            var L7_label2 = _context.Bricks.Select(s => new { code = s.L7_label2, status = s.L7_label2 }).Distinct().OrderBy(o => o.status).ToList();
            var CityType = _context.Bricks.Select(s => new { code = s.CityType, status = s.CityType }).Distinct().OrderBy(o => o.status).ToList();
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult_brick()
                {
                    L7_label2 = L7_label2,
                    CityType = CityType,
                    status = "ок",
                    Success = true
                }
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult Bricks_search(FilterBricks filter)
        {
            try
            {
                var _context = new GSContext(APP);


                var result = _context.Bricks.Select(s => s);
                if (!string.IsNullOrEmpty(filter.common))
                {
                    string[] vals = filter.common.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (vals.Length > 1)
                    {
                        result = result.Where(w => vals.AsQueryable().Where(v => w.L7_label.Contains(v)).Count() > 0);
                    }
                    else
                    {
                        result = result.Where(w =>
                            w.L2_label.Contains(filter.common)
                            || w.L3_label.Contains(filter.common)
                            || w.L4_label.Contains(filter.common)
                            || w.L5_label.Contains(filter.common)
                            || w.L6_label.Contains(filter.common)
                            || w.L7_label.Contains(filter.common)
                            );
                    }
                }
                if (!string.IsNullOrEmpty(filter.ids))
                {
                    result = result.Where(w =>
                        w.Id.Contains(filter.ids)
                        );
                }
                if (!string.IsNullOrEmpty(filter.post_index))
                {
                    result = result.Where(w =>
                        w.post_index.Contains(filter.post_index)
                        );
                }
                var result_list = result.ToList();
                var L7_list = result_list.Select(s => s.L7_label).Distinct().OrderBy(o => o).Select(s => new { value = s, label = s });
                var L6_list = result_list.Select(s => s.L6_label).Distinct().OrderBy(o => o).Select(s => new { value = s, label = s });
                var L5_list = result_list.Select(s => s.L5_label).Distinct().OrderBy(o => o).Select(s => new { value = s, label = s });
                var L4_list = result_list.Select(s => s.L4_label).Distinct().OrderBy(o => o).Select(s => new { value = s, label = s });
                var L3_list = result_list.Select(s => s.L3_label).Distinct().OrderBy(o => o).Select(s => new { value = s, label = s });
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult_bricks()
                    {
                        Data = result_list,
                        L7_list = L7_list,
                        L6_list = L6_list,
                        L5_list = L5_list,
                        L4_list = L4_list,
                        L3_list = L3_list,
                        count = result_list.Count(),
                        status = "ок",
                        Success = true
                    }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public FileResult Bricks_ToExcel()
        {
            /*
            ToDataTable<DataAggregator.Domain.Model.GS.GS>();
            ToDataTable<DataAggregator.Domain.Model.GS.GS_View_SP>();*/
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            var ret = _context.Bricks.ToList();


            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("Брики", 1, 1, ret, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "Брики.xlsx");
        }
        public int maxID(IQueryable<int> query, IQueryable<int> query2)
        {
            int _max = 1;
            if (query.Count() > 0)
                _max = query.Max();
            if (query2.Count() > 0)
            {
                if (query2.Max() > _max)
                    _max = query2.Max();
            }
            return _max;
        }
        [Authorize(Roles = "GS_Brick")]
        [HttpPost]
        public ActionResult Bricks_new(string Id, int level)
        {
            try
            {
                bool end_level = false;
                if (level == 50)
                {
                    level = 5;
                    end_level = true;
                }
                var _context = new GSContext(APP);
                var result = _context.Bricks.Where(w => w.Id == Id).Single();
                result.Id = "";
                result.comment = "";
                if (level <= 1)
                {
                    result.L1_label = "";
                    result.L1_id = _context.Bricks.Select(s => s.L1_id).Max() + 1;
                }
                result.Id += Convert.ToString(result.L1_id) + ".";
                if (level <= 2)
                {
                    result.L2_label = "";
                    result.L2_id = maxID(_context.Bricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L2_id), _context.changelogBricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L2_id)) + 1;
                }
                result.Id += Convert.ToString(result.L2_id) + ".";
                if (level <= 3)
                {
                    result.L3_label = "";
                    result.L3_id = maxID(_context.Bricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L3_id), _context.changelogBricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L3_id)) + 1;
                }
                result.Id += Convert.ToString(result.L3_id) + ".";
                if (level <= 4)
                {
                    result.L4_label = "";
                    result.L4_id = maxID(_context.Bricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L4_id), _context.changelogBricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L4_id)) + 1;
                }
                result.Id += Convert.ToString(result.L4_id) + ".";
                if (level <= 5)
                {
                    result.L5_label = "";
                    result.L5_id = maxID(_context.Bricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L5_id), _context.changelogBricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L5_id)) + 1;
                    result.CityType = "";
                }
                result.Id += Convert.ToString(result.L5_id) + ".";
                if (level <= 6)
                {
                    result.L6_label = "";
                    result.L6_id = maxID(_context.Bricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L6_id), _context.changelogBricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L6_id)) + 1;
                    if (end_level == true && level == 5 && result.L6_id < result.L5_id)
                    {
                        result.L6_id = result.L5_id;
                    }
                }
                result.Id += Convert.ToString(result.L6_id) + ".";
                if (level <= 7)
                {
                    result.L7_label = "";
                    result.L7_label2 = "";
                    result.post_index = "";
                    result.L7_id = maxID(_context.Bricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L7_id), _context.changelogBricks.Where(w => w.Id.StartsWith(result.Id)).Select(s => s.L7_id)) + 1;
                }
                result.Id += Convert.ToString(result.L7_id);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result, count = 1, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Brick")]
        [HttpPost]
        public ActionResult Bricks_Delete(ICollection<DataAggregator.Domain.Model.GS.Bricks> array)
        {
            var _context = new GSContext(APP);
            try
            {
                foreach (var item in array)
                {
                    _context.BrickDelete(item.Id);
                }
                //_context.SaveChanges();
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

        private bool CheckBricks(string JsonData)
        {
            using (var command = new SqlCommand())
            {
                var _context = new GSContext(APP);

                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)_context.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@JSONData", SqlDbType.NVarChar).Value = JsonData;

                command.CommandText = "dbo.CheckBricks_SP";

                _context.Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        [Authorize(Roles = "GS_Brick")]
        [HttpPost]
        public ActionResult Bricks_save(ICollection<DataAggregator.Domain.Model.GS.Bricks> array)
        {


            try
            {
                // преобразуем в JSON            
                string jsonBricks = JsonConvert.SerializeObject(array);

                // проверка на дубль
                CheckBricks(jsonBricks);

                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    if (item.CityType == null)
                        item.CityType = "";
                    if (item.L6_label == null)
                        item.L6_label = "";
                    if (item.L7_label == null)
                        item.L7_label = "";
                    if (item.L7_label2 == null)
                        item.L7_label2 = "";
                    if (item.comment == null)
                        item.comment = "";
                    if (item.Description == null)
                        item.Description = "";

                    var upd = _context.Bricks.Where(w => w.Id == item.Id).FirstOrDefault();
                    if (upd == null)//новая
                    {
                        upd = _context.Bricks.Add(item);
                    }

                    if (upd.comment != item.comment)
                    {
                        upd.comment = item.comment;
                    }

                    if (upd.L1_label != item.L1_label)
                    {
                        //_context.Bricks.Where(w => w.L1_id == item.L1_id).ToList().ForEach(u => u.L1_label = item.L1_label);
                        foreach (var u in _context.Bricks.Where(w => w.L1_id == item.L1_id))
                            u.L1_label = item.L1_label;
                    }
                    if (upd.L2_label != item.L2_label)
                    {
                        //_context.Bricks.Where(w => w.L2_id == item.L2_id && w.L1_id == item.L1_id).ToList().ForEach(u => u.L2_label = item.L2_label);
                        foreach (var u in _context.Bricks.Where(w => w.L2_id == item.L2_id && w.L1_id == item.L1_id))
                            u.L2_label = item.L2_label;
                    }
                    if (upd.L3_label != item.L3_label)
                    {
                        //_context.Bricks.Where(w => w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id).ToList().ForEach(u => u.L3_label = item.L3_label);

                        foreach (var u in _context.Bricks.Where(w => w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id))
                            u.L3_label = item.L3_label;
                    }
                    if (upd.L4_label != item.L4_label)
                    {
                        //_context.Bricks.Where(w => w.L4_id == item.L4_id && w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id).ToList().ForEach(u => u.L4_label = item.L4_label);
                        foreach (var u in _context.Bricks.Where(w => w.L4_id == item.L4_id && w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id))
                            u.L4_label = item.L4_label;
                    }
                    if (upd.L5_label != item.L5_label)
                    {
                        //_context.Bricks.Where(w => w.L5_id == item.L5_id && w.L4_id == item.L4_id && w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id).ToList().ForEach(u => u.L5_label = item.L5_label);
                        foreach (var u in _context.Bricks.Where(w => w.L5_id == item.L5_id && w.L4_id == item.L4_id && w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id))
                            u.L5_label = item.L5_label;
                    }
                    if (upd.CityType != item.CityType)
                    {
                        upd.CityType = item.CityType;
                        /*foreach (var u in _context.Bricks.Where(w => w.L6_id == item.L6_id && w.L5_id == item.L5_id && w.L4_id == item.L4_id && w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id))
                        {                            
                            u.L6_CityType = item.L6_CityType;
                        }*/
                    }
                    if (upd.L6_label != item.L6_label)
                    {
                        foreach (var u in _context.Bricks.Where(w => w.L6_id == item.L6_id && w.L5_id == item.L5_id && w.L4_id == item.L4_id && w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id))
                        {
                            u.L6_label = item.L6_label;
                        }
                    }
                    if (upd.L7_label2 != item.L7_label2 || upd.L7_label != item.L7_label || upd.post_index != item.post_index)
                    {
                        foreach (var u in _context.Bricks.Where(w => w.L7_id == item.L7_id && w.L6_id == item.L6_id && w.L5_id == item.L5_id && w.L4_id == item.L4_id && w.L3_id == item.L3_id && w.L2_id == item.L2_id && w.L1_id == item.L1_id))
                        {
                            u.L7_label = item.L7_label;
                            u.L7_label2 = item.L7_label2;
                            u.post_index = item.post_index;
                        }
                    }
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

        [Authorize(Roles = "GS_Brick")]
        [HttpPost]
        public ActionResult BrickRegion_Init()
        {
            var _context = new GSContext(APP);
            ViewData["L3"] = _context.Bricks.Where(w => w.L3_id > 0 && w.L4_id == 0).OrderBy(o => o.L3_label).ToList();
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult()
                {
                    Data = ViewData,
                    status = "ок",
                    Success = true
                }
            };
            return jsonNetResult;
        }
        [Authorize(Roles = "GS_Brick")]
        [HttpPost]
        public ActionResult BrickRegion_save(ICollection<DataAggregator.Domain.Model.GS.Bricks> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    if (item.Description == null)
                        item.Description = "";

                    var upd = _context.Bricks.Where(w => w.Id == item.Id).FirstOrDefault();

                    if (upd.Description != item.Description)
                    {
                        upd.Description = item.Description;
                    }


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



        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult base_address_search(filter_base_address filter)
        {
            try
            {
                var _context = new GSContext(APP);

                var result = _context.licenses_to_Use.Select(s => s);
                if (!string.IsNullOrEmpty(filter.common))
                {
                    result = result.Where(w =>
                    w.inn.Contains(filter.common)
                        ||
                        w.address.Contains(filter.common)
                        || w.full_name_licensee.Contains(filter.common)
                        || w.works.Contains(filter.common)
                        );
                }
                if (filter.toWork)
                {
                    result = result.Where(w => w.IsUse == true && w.UserAppendToGS == null);
                }
                if (filter.in_GS == 0)
                {
                    result = result.Where(w => w.GSId == null);
                }
                if (filter.in_GS == 1)
                {
                    result = result.Where(w => w.GSId > 0);
                }
                if (filter.isUse)
                {
                    result = result.Where(w => w.IsUse == true);
                }
                _context.Database.CommandTimeout = 0;
                int count_to_work = _context.licenses_to_Use.Where(w => w.IsUse == true && w.UserAppendToGS == null).Count();
                int count_not_in_GS = _context.licenses_to_Use.Where(w => w.GSId == null).Count();
                int count_in_GS = _context.licenses_to_Use.Where(w => w.GSId > 0).Count();
                int count_isUse = _context.licenses_to_Use.Where(w => w.IsUse == true).Count();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult_base_address()
                    {
                        Data = result.ToList(),
                        count = result.Count(),
                        count_to_work = count_to_work,
                        count_not_in_GS = count_not_in_GS,
                        count_in_GS = count_in_GS,
                        count_isUse = count_isUse,
                        status = "ок",
                        Success = true
                    }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult base_address_save(ICollection<DataAggregator.Domain.Model.GS.licenses_to_Use> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    if (item.address == null)
                        item.address = "";
                    if (item.index == null)
                        item.index = "";
                    if (item.region == null)
                        item.region = "";
                    if (item.city == null)
                        item.city = "";
                    if (item.street == null)
                        item.street = "";
                    if (item.BricksId == "")
                        item.BricksId = null;
                    if (item.GSId == 0)
                        item.GSId = null;
                    if (item.PharmacyId == 0)
                        item.PharmacyId = null;
                    var upd = _context.licenses_to_Use.Where(w => w.Id == item.Id).FirstOrDefault();
                    if (upd == null)//новая
                    {
                        //upd = _context.Bricks.Add(item);
                    }
                    else
                    {
                        //upd.address = item.address;
                        upd.index = item.index;
                        upd.region = item.region;
                        upd.city = item.city;
                        upd.street = item.street;
                        upd.BricksId = item.BricksId;
                        upd.IsUse = item.IsUse;
                        upd.PharmacyId = item.PharmacyId;
                        upd.GSId = item.GSId;
                    }
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


        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult base_address_To_GS()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.licenses_to_Use_To_GS(User.Identity.GetUserId());


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

        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult CreateLPUId()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.CreateLPUId(User.Identity.GetUserId());


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
        public FileResult base_address_ToExcel(ICollection<int> ids)
        {
            var _context = new GSContext(APP);
            var ret = _context.licenses_to_Use.Where(w => ids.Contains(w.Id)).ToList();
            /*.Select(s=>new { s.Id,s.inn,s.full_name_licensee,s.address,
                s.date,
                s.number,
                s.works,
                s.index,s.region,s.city,s.street,s.BricksId,s.PharmacyId,s.GSId}).ToList();*/
            Excel.Excel excel = new Excel.Excel();
            excel.Create();
            //System.Data.DataTable dt = Excel.Excel.ToDataTable(ret);

            excel.InsertDataTable("licenses_to_Use", 1, 1, ret, true, true, null);
            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "Шаблон.xlsx");
        }
        [HttpPost]
        public ActionResult base_address_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            Excel.Excel excel = new Excel.Excel();
            excel.Open(file.InputStream);

            var ret = excel.ToList<DataAggregator.Domain.Model.GS.licenses_to_Use>("licenses_to_Use", 1, 2, () => new Domain.Model.GS.licenses_to_Use());

            base_address_save(ret);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }


        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult licenses_to_Use_BrickIdSet()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.licenses_to_Use_BrickIdSet(User.Identity.GetUserId());


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

        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Spark_To(int TP)
        {
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            _context.Database.ExecuteSqlCommand("exec [dbo].[spark_interfax] " + TP.ToString());

            if (System.IO.File.Exists(@"c:\temp\spark_to.zip"))
            {
                System.IO.File.Delete(@"c:\temp\spark_to.zip");
            }
            ZipFile.CreateFromDirectory(@"\\s-sql2\Upload\Спарк\Туда", @"c:\temp\spark_to.zip");

            return File(@"c:\temp\spark_to.zip", "application/zip", "В спарк.zip");
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Spark_From(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");




            foreach (var delfile in System.IO.Directory.GetFiles(@"\\s-sql2\Upload\Спарк\Сюда\"))
            {
                if (System.IO.File.Exists(delfile))
                    System.IO.File.Delete(delfile);
            }
            foreach (var fileUP in uploads)
            {
                string filename = @"\\s-sql2\Upload\Спарк\Сюда\" + fileUP.FileName;
                fileUP.SaveAs(filename);
            }



            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            _context.Database.ExecuteSqlCommand("exec [dbo].[spark_interfax_ToServer]");

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult SpaSpark_Fromrk_To()
        {
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            _context.Database.ExecuteSqlCommand("exec [dbo].[spark_interfax_ToServer]");

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }

        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Organization_without_INN_search(Filter_Organization filter)
        {
            try
            {
                var _context = new GSContext(APP);
                string where = "";
                if (!string.IsNullOrEmpty(filter.inn))
                {
                    filter.inn = filter.inn.Replace(",", " ");
                    filter.inn = filter.inn.Trim();
                    if (where != "")
                        where += " and ";
                    if (filter.inn.Contains(" "))
                    {
                        where += " inn IN('" + filter.inn.Replace(" ", "','") + "')";
                    }
                    else
                    {
                        where += " inn like '%" + filter.inn + "%' ";
                    }
                }

                if (!string.IsNullOrEmpty(filter.ids))
                {
                    filter.ids = filter.ids.Replace(",", " ");
                    filter.ids = filter.ids.Trim();
                    if (where != "")
                        where += " and ";
                    where += " Id IN(" + filter.ids.Replace(" ", ",") + ")";
                }
                if (!string.IsNullOrEmpty(filter.common))
                {
                    if (where != "") where += " and ";
                    where += " (address like '%" + filter.common + "%' or EntityName like '%" + filter.common + "%') ";
                }
                if (where == "")
                    where = "1=1";
                _context.Database.CommandTimeout = 0;

                var result = _context.Database.SqlQuery<DataAggregator.Domain.Model.GS.Organization_without_INN>("select * from Organization_without_INN where " + where);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult_Organization()
                    {
                        Data = result.ToList(),
                        count = result.Count(),
                        count_to_work = 0,
                        count_in_GS = 0,
                        count_not_in_GS = 0,
                        count_withNull = 0,
                        count_IsNotCheck = 0,
                        count_IsErrors = 0,
                        count_in_LPU = 0,
                        count_in_NN = 0,
                        count_in_DO = 0,
                        status = "ок",
                        Success = true
                    }
                };
                return jsonNetResult;


            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Organization_without_INN_save(ICollection<DataAggregator.Domain.Model.GS.Organization> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    if (item.inn == null)
                        item.inn = "";
                    if (item.description == null)
                        item.description = "";
                    if (item.EntityName == null)
                        item.EntityName = "";
                    if (item.EntityType == null)
                        item.EntityType = "";
                    if (item.ogrn == null)
                        item.ogrn = "";
                    if (item.NetWorkName == null)
                        item.NetWorkName = "";
                    if (item.TypeOf == null)
                        item.TypeOf = "";
                    if (item.VidOf == null)
                        item.VidOf = "";
                    if (item.Brand == null)
                        item.Brand = "";
                    if (item.form == null)
                        item.form = "";
                    var upd = _context.Organization_without_INN.Where(w => w.Id == item.Id).Single();

                    upd.inn = item.inn;
                    upd.form = item.form;

                    upd.FIO = item.FIO;
                    upd.Status = item.Status;

                    upd.Boss_Name = item.Boss_Name;
                    upd.Date_registration = item.Date_registration;
                    upd.Date_licvidation = item.Date_licvidation;
                    upd.Vid_Action = item.Vid_Action;
                    upd.Phone = item.Phone;
                    upd.description = item.description;
                    upd.EntityName = item.EntityName;
                    upd.EntityType = item.EntityType;
                    upd.ogrn = item.ogrn;
                    upd.NetWorkName = item.NetWorkName;
                    upd.TypeOf = item.TypeOf;
                    upd.VidOf = item.VidOf;
                    upd.Brand = item.Brand;
                    upd.IsCheck = item.IsCheck;
                    upd.BricksId = item.BricksId;
                    upd.IsUseGS = item.IsUseGS;
                    upd.ActualId = item.ActualId;
                    upd.IsUseLPU = item.IsUseLPU;
                }
                _context.SaveChanges();
                _context.Organization_Without_INN_Set();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Organization_without_INN_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\_org_without_INN_up.xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.Organization_without_INN_FromExcel(@"S:\Upload\_org_without_INN_up.xlsx");

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }


        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Organization_search(Filter_Organization filter)
        {
            try
            {
                var _context = new GSContext(APP);
                string where = "";

                if (!string.IsNullOrEmpty(filter.inn))
                {
                    filter.inn = filter.inn.Replace(",", " ");
                    filter.inn = filter.inn.Trim();
                    if (where != "")
                        where += " and ";
                    if (filter.inn.Contains(" "))
                    {
                        where += " inn IN('" + filter.inn.Replace(" ", "','") + "')";
                    }
                    else
                    {
                        where += " inn like '%" + filter.inn + "%' ";
                    }
                }

                if (!string.IsNullOrEmpty(filter.ids))
                {
                    filter.ids = filter.ids.Replace(",", " ");
                    filter.ids = filter.ids.Trim();
                    if (where != "")
                        where += " and ";
                    where += " Id IN(" + filter.ids.Replace(" ", ",") + ")";
                }
                if (!string.IsNullOrEmpty(filter.common))
                {
                    if (where != "") where += " and ";
                    where += " (address like '%" + filter.common + "%' or EntityName like '%" + filter.common + "%') ";
                }

                if (filter.in_GS == 5)
                {
                    if (where != "") where += " and ";
                    where += " (IsLPU=1)";
                }
                if (filter.in_GS == 1)
                {
                    if (where != "") where += " and ";
                    where += " (inn in (select EntityINN from GS group by EntityINN)) ";
                }
                if (filter.toWork)
                {
                    if (where != "") where += " and ";
                    where += " (inn in (select inn from licenses_to_Use where IsUse=1 and UserAppendToGS is null group by inn)) ";
                }
                if (filter.in_GS == 5)
                {
                    if (filter.withNull)
                    {
                        if (where != "") where += " and ";
                        where += " (EntityName ='' or NetWorkName ='' or TypeOf ='' or VidOf ='' or Brand ='' or BricksId is null) ";
                    }
                }
                else
                {
                    if (filter.withNull)
                    {
                        if (where != "") where += " and ";
                        where += " (EntityName ='') ";
                    }
                }
                if (filter.in_GS == 10)//Дистрибьюторские отчёты
                {
                    if (where != "") where += " and ";
                    where += " (inn in (select EntityINN from DO_view group by EntityINN)) ";
                }
                if (filter.in_GS == 50)//сетях
                {
                    if (where != "") where += " and ";
                    where += " (inn in (select EntityINN from [spr_NetworkName] group by EntityINN)) ";
                }
                if (filter.in_GS == 0)
                {
                    if (where != "") where += " and ";
                    where += " (inn not in (select EntityINN from GS group by EntityINN)) ";
                }

                if (filter.IsNotCheck)
                {
                    if (where != "") where += " and ";
                    where += " (IsCheck=0) ";
                }
                if (filter.IsErrors)
                {
                    if (where != "") where += " and ";
                    where += @" (inn in (
select inn from(
select inn from[dbo].[Organization]
where ActualId is null
group by inn, EntityName Collate Cyrillic_General_CS_AS
)GG
group by inn having count(*) > 1

)) ";
                }
                if (where == "")
                    where = "1=1";
                _context.Database.CommandTimeout = 0;
                int count_to_work = _context.Database.SqlQuery<int>("select count(*) from Organization where inn in (select inn from licenses_to_Use where IsUse=1 and UserAppendToGS is null group by inn)").Single();
                int count_in_GS = _context.Database.SqlQuery<int>("select count(*) from Organization where inn in (select EntityINN from GS group by EntityINN)").Single();
                int count_in_LPU = _context.Database.SqlQuery<int>("select count(*) from Organization where IsLPU=1").Single();
                int count_in_DO = _context.Database.SqlQuery<int>("select count(*) from Organization where inn in (select EntityINN from DO_view group by EntityINN)").Single();
                int count_in_NN = _context.Database.SqlQuery<int>("select count(*) from Organization where inn in (select EntityINN from [dbo].[spr_NetworkName] group by EntityINN)").Single();

                int count_IsNotCheck = _context.Database.SqlQuery<int>("select count(*) from Organization where IsCheck=0").Single();

                int count_IsErrors = _context.Database.SqlQuery<int>(@"select count(*) from [dbo].[Organization] where inn in (
select inn from(
select inn from[dbo].[Organization]
where ActualId is null
group by inn, EntityName Collate Cyrillic_General_CS_AS
)GG
group by inn having count(*) > 1

)").Single();

                int count_not_in_GS = _context.Organization.Count() - count_in_GS;
                int count_withNull = _context.Organization.Where(w => w.EntityName == "").Count();


                var result = _context.Database.SqlQuery<DataAggregator.Domain.Model.GS.Organization>("select * from Organization where " + where);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult_Organization()
                    {
                        Data = result.ToList(),
                        count = result.Count(),
                        count_to_work = count_to_work,
                        count_in_GS = count_in_GS,
                        count_not_in_GS = count_not_in_GS,
                        count_withNull = count_withNull,
                        count_IsNotCheck = count_IsNotCheck,
                        count_IsErrors = count_IsErrors,
                        count_in_LPU = count_in_LPU,
                        count_in_NN = count_in_NN,
                        count_in_DO = count_in_DO,
                        status = "ок",
                        Success = true
                    }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Organization_save(ICollection<DataAggregator.Domain.Model.GS.Organization> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    if (item.description == null)
                        item.description = "";

                    if (item.EntityName == null)
                        item.EntityName = "";
                    if (item.EntityType == null)
                        item.EntityType = "";
                    if (item.ogrn == null)
                        item.ogrn = "";
                    if (item.NetWorkName == null)
                        item.NetWorkName = "";
                    if (item.TypeOf == null)
                        item.TypeOf = "";
                    if (item.VidOf == null)
                        item.VidOf = "";
                    if (item.Brand == null)
                        item.Brand = "";
                    var upd = _context.Organization.Where(w => w.Id == item.Id).Single();

                    upd.description = item.description;
                    upd.EntityName = item.EntityName;
                    upd.EntityType = item.EntityType;
                    upd.ogrn = item.ogrn;
                    upd.NetWorkName = item.NetWorkName;
                    upd.TypeOf = item.TypeOf;
                    upd.VidOf = item.VidOf;
                    upd.Brand = item.Brand;
                    upd.IsCheck = item.IsCheck;
                    upd.BricksId = item.BricksId;
                    upd.IsUseGS = item.IsUseGS;
                    upd.ActualId = item.ActualId;
                    upd.IsUseLPU = item.IsUseLPU;
                }
                _context.SaveChanges();
                _context.Organization_Set();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Organization_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\_org_up.xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.Organization_FromExcel(@"S:\Upload\_org_up.xlsx");

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        //Organization_BlockGS
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Organization_BlockGS(ICollection<string> inns, bool value)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var id in inns)
                {
                    var upd = _context.Organization.Where(w => w.inn == id).FirstOrDefault();
                    if (upd != null)
                    {
                        upd.IsUseGS = value;
                    }
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
                return BadRequest(e);
            }
        }

        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Calls_Add(List<int> GSIds)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var GSId in GSIds)
                {
                    _context.Calls.Add(new Domain.Model.GS.Calls { Creator_Date = DateTime.Now, Creator_User = User.Identity.GetUserName(), GSId = GSId, IsOpen = true });
                }

                _context.SaveChanges();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = GSIds, count = 1, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult GS_add_from_History_coding(int History_codingId)
        {
            try
            {
                var _context = new GSContext(APP);
                int GSId = _context.GS_add_from_History_coding(History_codingId);
                //int GSId = -50;
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = GSId, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "LPU_view")]
        [HttpPost]
        public ActionResult LPU_add_from_History_coding(int History_codingId)
        {
            try
            {
                var _context = new GSContext(APP);
                var Creator_User =  new Guid(User.Identity.GetUserId());
                int LPUId = _context.LPU_add_from_History_coding(History_codingId, Creator_User);             
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = LPUId, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        public ActionResult Point_Init()
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        List<int> ConvertToList(string S)
        {
            S = S.Replace("\r", " ");
            S = S.Replace("\n", " ");
            S = S.Replace(" ", ",");
            S = S.Replace(",,", ",");

            List<int> ret = new List<int>();
            foreach (string s1 in S.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ret.Add(Convert.ToInt32(s1));
            }
            return ret;
        }

        /// <summary>
        /// Поиск по аптекам
        /// </summary>
        /// <param name="IsNoKoord">наличие координат</param>
        /// <param name="PHids">Id аптеки</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Point_Search(bool IsNoKoord, string PHids)
        {
            try
            {
                var _context = new GSContext(APP);

                var l_PHids = !String.IsNullOrEmpty(PHids) ? String.Join(",", ConvertToList(PHids)) : String.Empty;

                var result = _context.Database.SqlQuery<GetPharmacys_Result>("dbo.GetPharmacys @PharmacyIdList, @IsNotExistCoordinates",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@PharmacyIdList", SqlDbType = System.Data.SqlDbType.VarChar, Value = l_PHids },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@IsNotExistCoordinates", SqlDbType = System.Data.SqlDbType.Bit, Value = IsNoKoord }
                ).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Reestr")]
        [HttpPost]
        public ActionResult Point_Save(ICollection<DataAggregator.Domain.Model.GS.Pharmacy> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    var UPD_0 = _context.Pharmacy.Where(w => w.PharmacyId == item.PharmacyId).FirstOrDefault();
                    if (UPD_0 != null)
                    {
                        UPD_0.fias_code_manual = item.fias_code_manual;
                        UPD_0.fias_id_manual = item.fias_id_manual;
                        UPD_0.geo_lat_manual = item.geo_lat_manual;
                        UPD_0.geo_lon_manual = item.geo_lon_manual;
                        UPD_0.IsChecked = item.IsChecked;
                    }
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
                return BadRequest(e);
            }
        }




        [Authorize(Roles = "GS_Summs")]
        public ActionResult SummsAlphaBit_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                var p = _context.GS_Period.Select(s => s.period).Distinct().OrderByDescending(o => o).ToList().Select(s => s.ToString("yyyy-MM"));
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = new GS_init_class() { periods = p }, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {

                return BadRequest(e);
            }
        }
        public List<string> GetPeriodsZA(DateTime period, int count)
        {
            List<string> ret = new List<string>();
            while (count >= 0)
            {
                ret.Add(period.ToString("MMM", new System.Globalization.CultureInfo("ru-RU")));
                period = period.AddMonths(-1);
                count--;
            }
            return ret;
        }
        public List<string> GetPeriodsAZ(DateTime period, int count)
        {
            List<string> ret = new List<string>();
            while (count >= 0)
            {
                ret.Add(period.ToString("MMM", new System.Globalization.CultureInfo("ru-RU")));
                period = period.AddMonths(1);
                count--;
            }
            return ret;
        }
        public List<string> GetPeriodListFilter(List<DateTime> list, int withQ)
        {
            List<string> ret = new List<string>();
            foreach (var item in list)
            {

                if (item.Month == 3 && withQ >= 1)
                {
                    ret.Add(item.ToString("yyyy-Q1"));
                }
                if (item.Month == 6 && withQ >= 1)
                {
                    ret.Add(item.ToString("yyyy-Q2"));
                }
                if (item.Month == 9 && withQ >= 1)
                {
                    ret.Add(item.ToString("yyyy-Q3"));
                }
                if (item.Month == 12 && withQ >= 1)
                {
                    ret.Add(item.ToString("yyyy-Q4"));
                }
                if (withQ <= 1)
                    ret.Add(item.ToString("yyyy-MM"));
            }
            return ret;
        }
        public List<DateTime> GetPeriodListFilter(string value)
        {
            List<DateTime> ret = new List<DateTime>();
            int year = Convert.ToInt32(value.Substring(0, 4));
            value = value.Substring(5);
            switch (value)
            {
                case "Q1":
                    ret.Add(new DateTime(year, 1, 15));
                    ret.Add(new DateTime(year, 2, 15));
                    ret.Add(new DateTime(year, 3, 15));
                    break;
                case "Q2":
                    ret.Add(new DateTime(year, 4, 15));
                    ret.Add(new DateTime(year, 5, 15));
                    ret.Add(new DateTime(year, 6, 15));
                    break;
                case "Q3":
                    ret.Add(new DateTime(year, 7, 15));
                    ret.Add(new DateTime(year, 8, 15));
                    ret.Add(new DateTime(year, 9, 15));
                    break;
                case "Q4":
                    ret.Add(new DateTime(year, 10, 15));
                    ret.Add(new DateTime(year, 11, 15));
                    ret.Add(new DateTime(year, 12, 15));
                    break;
                default:
                    ret.Add(new DateTime(year, Convert.ToInt32(value), 15));
                    break;
            }
            return ret;
        }
        [HttpPost]
        public ActionResult SummsAlphaBit_search(DateTime currentperiod, bool isDoubles, bool isDoublesAdd)
        {
            currentperiod = currentperiod.AddDays(14);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.Database.SqlQuery<Domain.Model.GS.AlphaBitSums_Period_SP>("dbo.AlphaBitSums_Period_SP @period,@isDoubles,@isDoublesAdd",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = currentperiod },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@isDoubles", SqlDbType = System.Data.SqlDbType.Bit, Value = isDoubles },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@isDoublesAdd", SqlDbType = System.Data.SqlDbType.Bit, Value = isDoublesAdd }
                ).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), Data2 = GetPeriodsZA(currentperiod, 13), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsAlphaBit_save(ICollection<DataAggregator.Domain.Model.GS.AlphaBitSums_Period_SP> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    item.NotNull();
                    if (item.Id > 0)
                    {
                        var UPD = _context.AlphaBitSums_Period.Where(w => w.Id == item.Id).Single();
                        UPD.IsUse = (bool)item.IsUse;
                        UPD.Comment = item.Comment;
                        UPD.LastSellingSum_IsUse = item.LastSellingSum_IsUse;
                        if (item.RealSellingSum != null)
                        {
                            UPD.RealSellingSum = (decimal)item.RealSellingSum;
                        }
                    }
                    else
                    {
                        if (item.Supplier == "Нео-Фарм (Москва)" && (item.IsUse ?? false) && (item.RealSellingSumFromFile ?? false))
                        {
                            var newsp = new DataAggregator.Domain.Model.GS.AlphaBitSums_Period();
                            newsp.Supplier = item.Supplier;
                            newsp.PharmacyId = item.PharmacyId;
                            newsp.Period = item.Period;
                            newsp.IsUse = item.IsUse ?? false;
                            newsp.RealSellingSum = item.RealSellingSum ?? (decimal)item.RealSellingSum;
                            newsp.Comment = item.Comment;
                            _context.AlphaBitSums_Period.Add(newsp);
                        }
                    }

                    //обновление прошлого периода -1
                    if (item.IsUse_p1 != null)
                    {
                        DateTime P1 = item.Period.AddMonths(-1);
                        var upd_p1 = _context.AlphaBitSums_Period.Where(w => w.PharmacyId == item.PharmacyId && w.Period == P1 && w.Supplier == item.Supplier).FirstOrDefault();
                        if (upd_p1 != null)
                        {
                            upd_p1.IsUse = (bool)item.IsUse_p1;
                            upd_p1.Comment = item.Comment_p1;
                        }
                    }
                    //обновление прошлого периода -2
                    if (item.IsUse_p2 != null)
                    {
                        DateTime P2 = item.Period.AddMonths(-2);
                        var upd_p2 = _context.AlphaBitSums_Period.Where(w => w.PharmacyId == item.PharmacyId && w.Period == P2 && w.Supplier == item.Supplier).FirstOrDefault();
                        if (upd_p2 != null)
                        {
                            upd_p2.IsUse = (bool)item.IsUse_p2;
                            upd_p2.Comment = item.Comment_p2;
                        }
                    }
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
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsAlphaBit_recalc(DateTime currentperiod)
        {
            try
            {
                var _context = new GSContext(APP);

                _context.AlphaBitSums_update(currentperiod.Year, currentperiod.Month);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult AlphaBitSums_from_Excel(IEnumerable<System.Web.HttpPostedFileBase> uploads, string supplier, string currentperiod)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    return null;

                var _context = new GSContext(APP);

                var file = uploads.First();
                string filename = @"\\s-sql2\Upload\AlphaBitSums_from_Excel_" + User.Identity.GetUserId() + ".xlsx";

                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                file.SaveAs(filename);

                _context.AlphaBitSums_from_Excel(filename, supplier, currentperiod);

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
        [Authorize(Roles = "GS_History")]
        public ActionResult History_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var spr_Source_client = _context.Database.SqlQuery<sprItemst>(@"select [Source_client] code,
[Source_client]+' - '+trim(str(sum(IIF(HC.GSid is null and DistrId is null and LPUId is null and status!=5,1,0)))) status from 
[adr].[History] H inner join adr.History_coding HC on HC.Id=H.CodingId
group by [Source_client] order by [Source_client]").ToList();
                spr_Source_client.Insert(0, new sprItemst() { Status = "все клиенты", code = "" });
                List<sprItem> spr_Tops = new List<sprItem>
                {
                    new sprItem() { code = 100, Status = "100" },
                    new sprItem() { code = 300, Status = "300" },
                    new sprItem() { code = 50000000, Status = "все строки" }
                };

                var spr_Comments = _context.Database.SqlQuery<sprItemst>("select Comments code,Comments+' - '+trim(str(COUNT(*))) status from [adr].History_coding where Comments<>'' group by Comments order by Comments").ToList();
                spr_Comments.Insert(0, new sprItemst() { Status = "<пусто>", code = "EmptyNull" });
                spr_Comments.Insert(0, new sprItemst() { Status = "все комментарии", code = "" });

                var spr_Category = _context.Database.SqlQuery<sprItem>("select cast(Id as int) as code,Value as Status from [adr].[History_Category] order by Status").ToList();
                spr_Category.Insert(0, new sprItem() { code = 0, Status = "все категории" });

                var spr_Status = _context.Database.SqlQuery<sprItem>(@"SELECT cast(HS.Id as int) as code, HS.Value+' - '+trim(str(COUNT(*))) AS Status
FROM            adr.History_Status HS INNER JOIN
                         adr.History_coding HC ON HS.Id = HC.Status
GROUP BY HS.Id, HS.Value
union
select -5,'Не Мусор'
").ToList();
                spr_Status.Insert(0, new sprItem() { code = 255, Status = "все статусы" });

                var spr_DataSource = _context.Database.SqlQuery<sprItemst>("select [DataSource] as code,DataSource+' - '+trim(str(COUNT(*))) status from [adr].[History_coding] where DataSource<>'' group by [DataSource] order by [DataSource]").ToList();
                spr_DataSource.Insert(0, new sprItemst() { Status = "все источники", code = "" });

                var spr_DataSourceType = _context.Database.SqlQuery<sprItemst>("select [DataSourceType] as code,DataSourceType+' - '+trim(str(COUNT(*))) status from [adr].[History_coding] where DataSourceType<>'' group by [DataSourceType] order by [DataSourceType]").ToList();
                spr_DataSourceType.Insert(0, new sprItemst() { Status = "все DataSourceType", code = "" });

                /*var spr_NetworkName = _context.Database.SqlQuery<sprItemst>("select [NetworkName] as code,NetworkName+' - '+trim(str(COUNT(*))) status from [adr].[History_coding] where NetworkName<>'' group by [NetworkName] order by [NetworkName]").ToList();
                spr_NetworkName.Insert(0, new sprItemst() { Status = "все сети", code = "" });
                */
                List<sprItemst> spr_Spec = _context.Database.SqlQuery<sprItemst>(@"
select 0 as id,cast('' as nvarchar(50)) as code,cast('без спец Фильтров' as nvarchar(50)) as Status
union
select 1,cast('noBrick' as nvarchar(50)) as code,cast('Нет бриков' as nvarchar(50)) +' - '+ltrim(str(count(*)))as Status
from [adr].[History_coding] where BricksId is null
union
select 2,cast('noTypeClients' as nvarchar(50)) as code,cast('Нет Тип Получателя' as nvarchar(50)) +' - '+ltrim(str(count(*))) as Status
from [adr].[History_coding] where TypeClients=''
union
select 3,cast('noGSId' as nvarchar(50)) as code,cast('Нет GSId' as nvarchar(50)) +' - '+ltrim(str(count(*))) as Status
from [adr].[History_coding] where GSId is null
union
select 4,cast('badGSId' as nvarchar(50)) as code,cast('Плохие GSId' as nvarchar(50)) +' - '+ltrim(str(count(*))) as Status
from [adr].[History_coding] where GSId>0 and GSId not in (select id from GS) and GSId<1000000000
union
select 5,cast('badAddress' as nvarchar(50)) as code,cast('Плохие Адреса' as nvarchar(50)) +' - '+ltrim(str(count(*))) as Status
from [adr].[History_coding] where CheckStat=1
union
select 6,cast('noLPU' as nvarchar(50)) as code,cast('Нет ЛПУ' as nvarchar(50)) +' - '+ltrim(str(count(*))) as Status
from [adr].[History_coding] where LPUId is null and status<>110

").ToList();


                var spr_userWork = _context.Database.SqlQuery<sprItemUser>("select [UserWork] code,[FullName]+' - '+trim(str(COUNT(*))) Status from [adr].[History_coding_inwork_View] group by [UserWork],[FullName]").ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult()
                    {
                        Data = new History_init_class()
                        {
                            spr_Source_client = spr_Source_client,
                            spr_Tops = spr_Tops,
                            spr_Category = spr_Category,
                            spr_DataSourceType = spr_DataSourceType,
                            spr_Spec = spr_Spec,
                            spr_DataSource = spr_DataSource,
                            // spr_NetworkName= spr_NetworkName,
                            spr_Comments = spr_Comments,
                            spr_Status = spr_Status,
                            spr_Users = spr_userWork
                        },
                        count = 0,
                        status = "ок",
                        Success = true
                    }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        //History_ReSync
        [Authorize(Roles = "GS_History")]
        public ActionResult History_ReSync()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                _context.Database.ExecuteSqlCommand("exec adr.Brick_Find_Set");
                _context.Database.ExecuteSqlCommand("exec adr.Organization_Find_Set");
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_History")]
        public ActionResult History_GetData(HystoryFilter filter, bool IsOnline)
        {
            try
            {
                filter.isnull();
                if (IsOnline == false)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    var _context = new GSContext(APP);

                    _context.adr_GetData(userGuid, filter.top, filter.Source_client, filter.text, filter.GSIDs, filter.PharmacyIDs, filter.Ids, filter.Category, filter.Status, filter.Comments,
                        filter.INN, filter.Address, filter.DataSource, filter.NetworkName, filter.Spec, filter.DataSourceType, IsOnline);
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_History")]
        [HttpPost]
        public ActionResult History_SetData(ICollection<DataAggregator.Domain.Model.GS.History_coding_inwork_View> array, int level, bool IsOnline, bool IsGSwork, bool IsTypeClient)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());
                var _context = new GSContext(APP);
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        if (IsOnline)
                        {
                            var upd = _context.History_coding.Where(w => w.Id == item.Id).Single();
                            if (IsTypeClient == true)
                            {
                                if (upd.TypeClients != item.TypeClients)
                                {
                                    upd.TypeClients = item.TypeClients ?? "";
                                    upd.TypeClients_When = DateTime.Now;
                                    upd.TypeClients_Who = item.TypeClients_Who;
                                }
                            }
                        }
                        else
                        {
                            var upd_inwork = _context.History_coding_inwork.Where(w => w.Id == item.Id).Single();
                            upd_inwork.GSId = item.GSId;
                            upd_inwork.LPUId = item.LPUId;
                            upd_inwork.DistrId = item.DistrId;
                            upd_inwork.PharmacyId = item.PharmacyId;
                            upd_inwork.BricksId = item.BricksId;

                            upd_inwork.Category = item.Category;
                            upd_inwork.NetworkName = item.NetworkName ?? "";
                            upd_inwork.Address_region = item.Address_region ?? "";
                            upd_inwork.Address_city = item.Address_city ?? "";
                            upd_inwork.Address_street = item.Address_street ?? "";
                            upd_inwork.EntityINN = item.EntityINN ?? "";
                            upd_inwork.EntityName = item.EntityName ?? "";
                            upd_inwork.PharmacyBrand = item.PharmacyBrand ?? "";
                            upd_inwork.Comments = item.Comments ?? "";
                            upd_inwork.Spark = item.Spark ?? "";
                            upd_inwork.Spark2 = item.Spark2 ?? "";
                            upd_inwork.Status = item.Status;

                            upd_inwork.CheckStat = item.CheckStat;
                        }
                    }
                }
                _context.SaveChanges();
                if (level == 1)
                {
                    _context.adr_SetData(userGuid);
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_History")]
        [HttpPost]
        public ActionResult History_SetOtherData(Guid UsersCL)
        {
            try
            {
                var _context = new GSContext(APP);

                _context.adr_SetData(UsersCL);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_History")]
        public ActionResult History_ShowData(HystoryFilter filter, bool IsOnline)
        {
            filter.isnull();
            var userGuid = new Guid(User.Identity.GetUserId());
            //userGuid = new Guid("c511f28b-fe1a-4f65-a72d-c7cd5bd7182a");//чтобы проверять типо под кем-то
            List<History_coding_inwork_View> result = null;
            var _context = new GSContext(APP);
            if (IsOnline == false)
            {
                _context.Database.CommandTimeout = 0;
                _context.SyncWithSPR_inwork();
                result = _context.History_coding_inwork_View.Where(w => w.UserWork == userGuid).Take(200000).ToList();
            }
            else
            {
                _context.Database.CommandTimeout = 20 * 60;
                result = _context.adr_GetData(userGuid, filter.top, filter.Source_client, filter.text, filter.GSIDs, filter.PharmacyIDs, filter.Ids, filter.Category, filter.Status, filter.Comments,
                    filter.INN, filter.Address, filter.DataSource, filter.NetworkName, filter.Spec, filter.DataSourceType, IsOnline);
            }
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [Authorize(Roles = "GS_History")]
        public ActionResult History_GetClassifier(byte Category, string text, int GSId, int LPUId, int DistrId, int PharmacyId, string BricksId, string INN, string NetworkName)
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            var _context = new GSContext(APP);
            if (Category == 10)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = "%" + text.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_GS_view.Where(w => System.Data.Entity.DbFunctions.Like(w.Address, text) || System.Data.Entity.DbFunctions.Like(w.Address_city, text) || System.Data.Entity.DbFunctions.Like(w.Address_street, text));
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(BricksId))
                {
                    BricksId = "%" + BricksId.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_GS_view.Where(w => System.Data.Entity.DbFunctions.Like(w.BricksId, BricksId));
                    jsonNetResult.Data = result.ToList();
                }
                if (GSId > 0 && GSId < 1000000000)//ГС
                {
                    var result = _context.History_SPR_GS_view.Where(w => w.GSId == GSId);
                    jsonNetResult.Data = result.ToList();
                }
                if (GSId > 1000000000)//Неполный ГС
                {
                    var result = _context.History_SPR_BigGS_view.Where(w => w.GSId == GSId);
                    jsonNetResult.Data = result.ToList();
                }
                if (PharmacyId > 0)
                {
                    var result = _context.History_SPR_GS_view.Where(w => w.PharmacyId == PharmacyId);
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(INN))
                {
                    INN = "%" + INN.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_GS_view.Where(w => System.Data.Entity.DbFunctions.Like(w.EntityINN, INN));
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(NetworkName))
                {
                    var result = _context.History_SPR_GS_view.Where(w => w.NetworkName == NetworkName);
                    jsonNetResult.Data = result.ToList();
                }
            }
            if (Category == 30)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = "%" + text.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_LPU_view.Where(w => System.Data.Entity.DbFunctions.Like(w.Address, text));
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(BricksId))
                {
                    BricksId = "%" + BricksId.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_LPU_view.Where(w => System.Data.Entity.DbFunctions.Like(w.BricksId, BricksId));
                    jsonNetResult.Data = result.ToList();
                }
                if (LPUId > 0 && LPUId < 1000000000)//ГС
                {
                    var result = _context.History_SPR_LPU_view.Where(w => w.LPUId == LPUId);
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(INN))
                {
                    INN = "%" + INN.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_LPU_view.Where(w => System.Data.Entity.DbFunctions.Like(w.EntityINN, INN));
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(NetworkName))
                {
                    var result = _context.History_SPR_LPU_view.Where(w => w.NetworkName == NetworkName);
                    jsonNetResult.Data = result.ToList();
                }
            }
            if (Category == 20)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = "%" + text.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_Distr_view.Where(w => System.Data.Entity.DbFunctions.Like(w.Address, text));
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(BricksId))
                {
                    BricksId = "%" + BricksId.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_Distr_view.Where(w => System.Data.Entity.DbFunctions.Like(w.BricksId, BricksId));
                    jsonNetResult.Data = result.ToList();
                }
                if (DistrId > 0 && DistrId < 1000000000)//ГС
                {
                    var result = _context.History_SPR_Distr_view.Where(w => w.DistrId == DistrId);
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(INN))
                {
                    INN = "%" + INN.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_Distr_view.Where(w => System.Data.Entity.DbFunctions.Like(w.EntityINN, INN));
                    jsonNetResult.Data = result.ToList();
                }
                if (!string.IsNullOrEmpty(NetworkName))
                {
                    var result = _context.History_SPR_Distr_view.Where(w => w.NetworkName == NetworkName);
                    jsonNetResult.Data = result.ToList();
                }
            }
            if (Category == 201 && !string.IsNullOrEmpty(text))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = "%" + text.Replace(" ", "%") + "%";
                    var result = _context.History_SPR_Brick_view.Where(w => System.Data.Entity.DbFunctions.Like(w.Address, text));
                    jsonNetResult.Data = result.ToList();
                }
            }
            return jsonNetResult;
        }
        [HttpPost]
        public FileResult History_ToExcel()
        {
            var userGuid = new Guid(User.Identity.GetUserId());
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            var ret = _context.History_coding_inwork_View.Where(w => w.UserWork == userGuid).ToList();


            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("History_coding", 1, 1, ret, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "History_coding.xlsx");
        }
        [Authorize(Roles = "GS_History")]
        [HttpPost]
        public ActionResult History_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\History_" + User.Identity.GetUserId() + ".xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.History_from_Excel(User.Identity.GetUserId());

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [Authorize(Roles = "GS_Summs")]
        public ActionResult SummsPeriod_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                var p = _context.GS_Period.Where(w => w.Summa_Start > 0).Select(s => s.period).Distinct().OrderByDescending(o => o).Take(4).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = new GS_init_class() { periods = GetPeriodListFilter(p, 0) }, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SummsPeriod_Search(string currentperiod, bool IsNoSumms, bool IsNeed, string FilterP, string NetworkName)
        {

            try
            {
                List<DateTime> periods = GetPeriodListFilter(currentperiod);
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.Database.SqlQuery<Domain.Model.GS.SummsPeriod_SP>("dbo.[SummsPeriod_SP] @period,@IsNoSumms,@IsNeed,@FilterP,@NetworkName",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@IsNoSumms", SqlDbType = System.Data.SqlDbType.Bit, Value = IsNoSumms },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@IsNeed", SqlDbType = System.Data.SqlDbType.Bit, Value = IsNeed },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@FilterP", SqlDbType = System.Data.SqlDbType.NVarChar, Value = FilterP },
                     new System.Data.SqlClient.SqlParameter { ParameterName = "@NetworkName", SqlDbType = System.Data.SqlDbType.NVarChar, Value = NetworkName }
                    ).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), Data2 = GetPeriodsZA(periods.Max(), 13), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsPeriod_AlphaBitSums_Set(string currentperiod)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                //foreach (var per in periods)
                //{
                _context.Database.ExecuteSqlCommand("dbo.[SummsPeriod_Set_AlphaBitSums] @period",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() });
                //}
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsPeriod_OFD_Set(string currentperiod)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                foreach (var per in periods)
                {
                    _context.SummsPeriod_OFD_Apply(per);
                }


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsPeriod_Save(ICollection<DataAggregator.Domain.Model.GS.SummsPeriod_SP> array, int count_period_edit)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    item.NotNull();
                    if (item.GSId > 0)
                    {
                        //Если обновляем, то не забываем что надо сумму пометить как ручную иначе она затрётся.
                        var UPD_0 = _context.GS_Period.Where(w => w.GSId == item.GSId && w.period == item.Period).Single();
                        DateTime p_1 = item.Period.AddMonths(-1);
                        DateTime p_2 = item.Period.AddMonths(-2);
                        var UPD_1 = _context.GS_Period.Where(w => w.GSId == item.GSId && w.period == p_1).Single();
                        var UPD_2 = _context.GS_Period.Where(w => w.GSId == item.GSId && w.period == p_2).Single();

                        item.Summa_Start_p0 = item.Summa_Start_p0 > 0 ? item.Summa_Start_p0 : 0;

                        UPD_0.isExists = item.isExists_p0;
                        if (item.Summa_Start_p0 > 0 && item.Summa_Start_p0 != UPD_0.Summa_Start)
                        {
                            item.SourceData_p0 = "Fix";
                            UPD_0.Summa_Start = (decimal)item.Summa_Start_p0;
                            UPD_0.Summa_Region = (decimal)item.Summa_Start_p0;
                            UPD_0.Summa = (decimal)item.Summa_Start_p0;
                        }
                        UPD_0.NetworkName = item.NetworkName_p0;
                        UPD_0.SourceData = item.SourceData_p0;
                        if (count_period_edit > 0)
                        {
                            item.Summa_Start_p1 = item.Summa_Start_p1 > 0 ? item.Summa_Start_p1 : 0;
                            item.Summa_Start_p2 = item.Summa_Start_p2 > 0 ? item.Summa_Start_p2 : 0;
                            if (item.Summa_Start_p1 > 0 && item.Summa_Start_p1 != UPD_1.Summa_Start)
                            {
                                item.SourceData_p1 = "Fix";
                                UPD_1.Summa_Start = (decimal)item.Summa_Start_p1;
                                UPD_1.Summa_Region = (decimal)item.Summa_Start_p1;
                                UPD_1.Summa = (decimal)item.Summa_Start_p1;
                            }
                            if (item.Summa_Start_p2 > 0 && item.Summa_Start_p2 != UPD_2.Summa_Start)
                            {
                                item.SourceData_p2 = "Fix";
                                UPD_2.Summa_Start = (decimal)item.Summa_Start_p2;
                                UPD_2.Summa_Region = (decimal)item.Summa_Start_p2;
                                UPD_2.Summa = (decimal)item.Summa_Start_p2;
                            }

                            UPD_1.isExists = item.isExists_p1;
                            UPD_1.NetworkName = item.NetworkName_p1;
                            UPD_1.SourceData = item.SourceData_p1;

                            UPD_2.isExists = item.isExists_p2;
                            UPD_2.NetworkName = item.NetworkName_p2;
                            UPD_2.SourceData = item.SourceData_p2;
                        }
                    }
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
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsPeriod_recalc(string currentperiod)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                int period_count = 1;
                //foreach (var per in periods)
                //{
                //    if (per.Month == 3 || per.Month == 6 || per.Month == 9 || per.Month == 12)
                //        period_count = periods.Count();
                //    else
                //        period_count = 1;
                //    _context.Database.ExecuteSqlCommand("dbo.[SummsPeriod_recalc] @period,@period_count", 
                //        new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = per },
                //        new System.Data.SqlClient.SqlParameter { ParameterName = "@period_count", SqlDbType = System.Data.SqlDbType.Int, Value = period_count });
                //}
                _context.Database.ExecuteSqlCommand("dbo.[SummsPeriod_recalc] @period,@period_count",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period_count", SqlDbType = System.Data.SqlDbType.Int, Value = period_count });
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public FileResult SummsPeriod_To_Excel(string currentperiod, bool IsNoSumms, bool IsNeed, string FilterP, string NetworkName)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            var result = _context.Database.SqlQuery<Domain.Model.GS.SummsPeriod_SP>("dbo.[SummsPeriod_SP] @period,@IsNoSumms,@IsNeed,@FilterP,@NetworkName",
                new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() },
                new System.Data.SqlClient.SqlParameter { ParameterName = "@IsNoSumms", SqlDbType = System.Data.SqlDbType.Bit, Value = IsNoSumms },
                new System.Data.SqlClient.SqlParameter { ParameterName = "@IsNeed", SqlDbType = System.Data.SqlDbType.Bit, Value = IsNeed },
                new System.Data.SqlClient.SqlParameter { ParameterName = "@FilterP", SqlDbType = System.Data.SqlDbType.NVarChar, Value = FilterP },
                 new System.Data.SqlClient.SqlParameter { ParameterName = "@NetworkName", SqlDbType = System.Data.SqlDbType.NVarChar, Value = NetworkName }
                ).ToList();


            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("SummsPeriod", 1, 1, result, true, true, null);

            excel.Style_ColumnBackColor("SummsPeriod", 1, System.Drawing.Color.Red);

            excel.Style_ColumnBackColor("SummsPeriod", 12, System.Drawing.Color.Yellow);
            excel.Style_ColumnBackColor("SummsPeriod", 25, System.Drawing.Color.Yellow);
            excel.Style_ColumnBackColor("SummsPeriod", 37, System.Drawing.Color.Yellow);
            if (periods.Count() > 1)
            {
                excel.Style_ColumnBackColor("SummsPeriod", 13, System.Drawing.Color.Yellow);
                excel.Style_ColumnBackColor("SummsPeriod", 25, System.Drawing.Color.Yellow);
                excel.Style_ColumnBackColor("SummsPeriod", 37, System.Drawing.Color.Yellow);

                excel.Style_ColumnBackColor("SummsPeriod", 14, System.Drawing.Color.Yellow);
                excel.Style_ColumnBackColor("SummsPeriod", 26, System.Drawing.Color.Yellow);
                excel.Style_ColumnBackColor("SummsPeriod", 38, System.Drawing.Color.Yellow);
            }

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "SummsPeriod_" + currentperiod + ".xlsx");
        }
        [HttpPost]
        public ActionResult SummsPeriod_from_Excel(IEnumerable<System.Web.HttpPostedFileBase> uploads, string currentperiod)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\SummsPeriod_from_Excel_" + User.Identity.GetUserId() + ".xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.SummsPeriod_from_Excel(User.Identity.GetUserId(), currentperiod);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [Authorize(Roles = "GS_Summs")]
        public ActionResult SummsRegion_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                var p = _context.GS_Period.Where(w => w.Summa_Start > 0).Select(s => s.period).Distinct().OrderByDescending(o => o).Take(12).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = new GS_init_class() { periods = GetPeriodListFilter(p, 0) }, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SummsRegion_Search(string currentperiod, decimal K_otkl)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                if (K_otkl > 0)
                {
                    var upd = _context.GS_Period_Region.Where(w => periods.Contains(w.Period));
                    foreach (var u1 in upd)
                    {
                        u1.K_otkl = K_otkl;
                    }
                    _context.SaveChanges();
                }
                var result = _context.Database.SqlQuery<Domain.Model.GS.SummsPeriod_Region_SP>("dbo.[SummsPeriod_Region_SP] @period,@flag,@period_count",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@flag", SqlDbType = System.Data.SqlDbType.NChar, Value = "" },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period_count", SqlDbType = System.Data.SqlDbType.Int, Value = periods.Count() }
                    ).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), Data2 = GetPeriodsZA(periods.Max(), 13), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsRegion_Save(ICollection<DataAggregator.Domain.Model.GS.SummsPeriod_Region_SP> array, int count_period_edit)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    var UPD_0 = _context.GS_Period_Region.Where(w => w.Region == item.Region && w.Period == item.Period).FirstOrDefault();
                    if (UPD_0 != null)
                    {
                        UPD_0.Kof = item.Kof;
                    }
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
                return BadRequest(e);
            }
        }


        [Authorize(Roles = "GS_Summs")]
        public ActionResult SummsNetwork_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                var p = _context.GS_Period.Where(w => w.Summa_Start > 0).Select(s => s.period).Distinct().OrderByDescending(o => o).Take(28).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = new GS_init_class() { periods = GetPeriodListFilter(p, 0) }, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SummsNetwork_Search(string currentperiod, bool IsWithAnket)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.Database.SqlQuery<Domain.Model.GS.SummsPeriod_Network_SP>("dbo.[SummsPeriod_Network_SP] @period,@IsWithAnket,@period_count",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@IsWithAnket", SqlDbType = System.Data.SqlDbType.Bit, Value = IsWithAnket },
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period_count", SqlDbType = System.Data.SqlDbType.Int, Value = periods.Count() }
                    ).ToList();
                DateTime period_jan = new DateTime(periods.Max().Year, 1, 15);
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), Data2 = GetPeriodsAZ(period_jan.AddMonths(-1), 12), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Authorize(Roles = "GS_Summs")]
        public ActionResult SummsAnket_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                var p = _context.GS_Period.Where(w => w.Summa_Start > 0).Select(s => s.period).Distinct().OrderByDescending(o => o).Take(12).ToList();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = new GS_init_class() { periods = GetPeriodListFilter(p, 0) }, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SummsAnket_Search(string currentperiod)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                DateTime period_jan = new DateTime(periods.Max().Year, 1, 15);
                var result = _context.Database.SqlQuery<Domain.Model.GS.SummsPeriod_Anket_SP>("dbo.[SummsPeriod_Anket_SP] @period",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() }
                    ).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), Data2 = GetPeriodsAZ(period_jan.AddMonths(-1), 12), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsAnket_Save(ICollection<DataAggregator.Domain.Model.GS.SummsPeriod_Anket_SP> array, int count_period_edit)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    var UPDs_3 = _context.GS_Period_Network.Where(w => w.NetworkName == item.NetworkName && w.Region == item.Region &&
 w.Period == item.Period).FirstOrDefault();
                    if (UPDs_3 != null)
                        UPDs_3.Kof = (double)item.Kof;
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
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SummsAnket_FromTemplate(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\SummsAnket_FromTemplate.xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.SummsAnket_FromTemplate(@"S:\Upload\SummsAnket_FromTemplate.xlsx");
            /*Excel.Excel excel = new Excel.Excel();
            excel.Open(file.InputStream);
            */
            //var row_U = excel.ToList<Domain.Model.GS.GS_View_SP>("ГС", 1, 2, () => new Domain.Model.GS.GS_View_SP());

            //return GS_save(row_U);
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [HttpPost]
        public FileResult SummsAnket_ToTemplate(string currentperiod)
        {
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            DateTime period = periods.Max();

            var ret = _context.GS_Period_Network_Anket.Where(w => w.Period == period).ToList();

            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("Анкеты", 1, 1, ret, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "SummsAnket_ToTemplate.xlsx");
        }




        [Authorize(Roles = "GS_Summs")]
        public ActionResult SummsOFD_Init()
        {
            try
            {
                var _context = new GSContext(APP);
                var p = _context.GS_Period.Select(s => s.period).Distinct().OrderByDescending(o => o).ToList().Select(s => s.ToString("yyyy-MM"));
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = new GS_init_class() { periods = p }, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SummsOFD_search(DateTime currentperiod)
        {
            currentperiod = currentperiod.AddDays(14);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.Database.SqlQuery<Domain.Model.GS.SummsPeriod_OFD_SP>("dbo.SummsPeriod_OFD_SP @period",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = currentperiod }

                    ).ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), Data2 = GetPeriodsZA(currentperiod, 13), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsOFD_save(ICollection<DataAggregator.Domain.Model.GS.SummsPeriod_OFD_SP> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    item.NotNull();
                    if (item.Id > 0)
                    {
                        var UPD = _context.OFDSumms_Period.Where(w => w.Id == item.Id).Single();
                        UPD.IsUse = (bool)item.IsUse;
                        UPD.Comment = item.Comment;
                        UPD.Kof_A1 = item.Kof_A1;
                        if (UPD.Kof_Brick != item.Kof_Brick)
                        {
                            var UPD_Kof_Brick = _context.OFDSumms_Period.Where(w => w.Period == UPD.Period && w.BrickId == UPD.BrickId);
                            foreach (var itemkb in UPD_Kof_Brick)
                            {
                                itemkb.Kof_Brick = item.Kof_Brick;
                            }
                        }
                    }
                    ////обновление прошлого периода -1
                    //if (item.IsUse_p1 != null)
                    //{
                    //    DateTime P1 = item.Period.AddMonths(-1);
                    //    var upd_p1 = _context.SummsOFD_Period.Where(w => w.PharmacyId == item.PharmacyId && w.Period == P1 && w.Supplier == item.Supplier).FirstOrDefault();
                    //    upd_p1.IsUse = (bool)item.IsUse_p1;
                    //    upd_p1.Comment = item.Comment_p1;
                    //}
                    ////обновление прошлого периода -2
                    //if (item.IsUse_p2 != null)
                    //{
                    //    DateTime P2 = item.Period.AddMonths(-2);
                    //    var upd_p2 = _context.SummsOFD_Period.Where(w => w.PharmacyId == item.PharmacyId && w.Period == P2 && w.Supplier == item.Supplier).FirstOrDefault();
                    //    upd_p2.IsUse = (bool)item.IsUse_p2;
                    //    upd_p2.Comment = item.Comment_p2;
                    //}
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
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult SummsOFD_recalc(DateTime currentperiod)
        {
            try
            {
                var _context = new GSContext(APP);
                currentperiod = currentperiod.AddDays(14);
                //_context.SummsPeriod_OFD_Update(currentperiod);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SummsOFD_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\SummsOFD_" + User.Identity.GetUserId() + ".xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.SummsOFD_FromExcel(User.Identity.GetUserId());

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult SummsPeriod_SummsOFD_Set(string currentperiod)
        {
            List<DateTime> periods = GetPeriodListFilter(currentperiod);
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                //foreach (var per in periods)
                //{
                _context.Database.ExecuteSqlCommand("dbo.[SummsPeriod_Set_SummsOFD] @period",
                    new System.Data.SqlClient.SqlParameter { ParameterName = "@period", SqlDbType = System.Data.SqlDbType.Date, Value = periods.Max() });
                //}
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult DistributorBranch_Init()
        {
            try
            {

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult DistributorBranch_search()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.DistributorBranch.ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult DistributorBranch_save(ICollection<DataAggregator.Domain.Model.GS.DistributorBranch> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    item.NotNull();
                    if (item.Id > 0)
                    {
                        var UPD = _context.DistributorBranch.Where(w => w.Id == item.Id).Single();
                        UPD.EntityINN = item.EntityINN;
                        UPD.EntityName = item.EntityName;
                        UPD.DistributorBrand = item.DistributorBrand;
                        UPD.Name_Short = item.Name_Short;
                        UPD.Address_city = item.Address_city;
                        UPD.Address_city_All = item.Address_city_All;
                        UPD.Email = item.Email;
                        UPD.Phone = item.Phone;
                        UPD.Address_street = item.Address_street;
                        UPD.Web = item.Web;
                        UPD.Comment = item.Comment;
                    }
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
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public FileResult DistributorBranch_To_Excel()
        {
            var _context = new GSContext(APP);
            _context.Database.CommandTimeout = 0;
            var result = _context.DistributorBranch.ToList();


            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("SummsPeriod", 1, 1, result, true, true, null);
            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "SummsPeriod.xlsx");
        }
        [Authorize(Roles = "GS_Summs")]
        [HttpPost]
        public ActionResult DistributorBranch_from_Excel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql2\Upload\DistributorBranch_from_Excel_" + aspUser.UserId.ToString() + ".xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new GSContext(APP);
            _context.DistributorBranch_from_Excel(aspUser.UserId.ToString());

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }


        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult spr_OperationMode_Init()
        {
            try
            {

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult spr_OperationMode_search()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                _context.Database.ExecuteSqlCommand(@"delete from dbo.spr_OperationMode where OperationMode not in (select OperationMode from dbo.GS group by OperationMode)
and Monday is null and [Tuesday] is null and [Wednesday] is null and [Thursday] is null and [Friday] is null and [Saturday] is null and [Sunday] is null

insert into dbo.spr_OperationMode (OperationMode)
select OperationMode from dbo.GS
where OperationMode not in (select OperationMode from dbo.spr_OperationMode)
group by OperationMode
");
                // var result = _context.spr_OperationMode.OrderBy(o=>o.OperationMode).ToList();
                var result = _context.Database.SqlQuery<spr_OperationMode>("select * from dbo.spr_OperationMode where OperationMode in (select OperationMode from dbo.GS)");
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = result.ToList(), count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult spr_OperationMode_save(ICollection<DataAggregator.Domain.Model.GS.spr_OperationMode> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    var UPD = _context.spr_OperationMode.Where(w => w.OperationMode == item.OperationMode).Single();
                    UPD.Monday = item.Monday;
                    UPD.Tuesday = item.Tuesday;
                    UPD.Wednesday = item.Wednesday;
                    UPD.Thursday = item.Thursday;
                    UPD.Friday = item.Friday;
                    UPD.Saturday = item.Saturday;
                    UPD.Sunday = item.Sunday;
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
                return BadRequest(e);
            }
        }


        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult NetworkBrand_Init()
        {
            try
            {

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult NetworkBrand_Search()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.Database.SqlQuery<NetworkBrandView>("exec dbo.NetworkBrand_SP");
                ViewData["NetworkBrand"] = result.ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewData, count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult NetworkBrand_Save(ICollection<DataAggregator.Domain.Model.GS.NetworkBrandView> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    if (item.Comment == null) item.Comment = "";
                    if (item.PharmacyBrand == null) item.PharmacyBrand = "";
                    if (item.NetworkName == null) item.NetworkName = "";

                    var UPD = _context.NetworkBrand.Where(w => w.Id == item.Id).Single();
                    UPD.Comment = item.Comment;
                    UPD.Used = item.Used;
                    if (UPD.PharmacyBrand != item.PharmacyBrand)
                    {
                        _context.SaveChanges();
                        _context.NetworkBrand_UpdateBrand(UPD.NetworkName, UPD.PharmacyBrand, item.PharmacyBrand);
                    }
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
                return BadRequest(e);
            }
        }


        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult Network_Init()
        {
            try
            {

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult Network_Search()
        {
            try
            {
                var _context = new GSContext(APP);
                _context.Database.CommandTimeout = 0;
                var result = _context.Database.SqlQuery<spr_NetworkNameView>("exec dbo.spr_NetworkName_SP").ToList();
                var period_cur = result[0].Period;
                var period_cur_from = period_cur.AddMonths(-3);
                /*var NNP = _context.spr_NetworkName_Period.Where(w=>w.Rx_Share>0||w.AverageReceipt>0||w.BAD_Share>0||w.Ecom_Share>0||
                w.OTC_Share>0|| w.Other_Share>0||w.PreferentialRecipesSalesSum>0||w.SKUTotalCount>0||w.STM_Share>0||w.TotalSalesSum>0).
                    Where(w => w.period < period_cur && w.period >= period_cur_from).OrderBy(o => o.Id).OrderBy(o => o.period).ToList() ;

                foreach (var item in result)
                {
                    item.spr_NetworkName_Periods = NNP.Where(w => w.spr_NetworkNameId == item.Id).ToList();
                }*/
                ViewData["spr_NetworkNameView"] = result;
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewData, count = result.Count(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult Network_Save(ICollection<DataAggregator.Domain.Model.GS.spr_NetworkNameView> array)
        {
            try
            {
                var _context = new GSContext(APP);

                foreach (var item in array)
                {
                    if (item.Associations == null) item.Associations = "";
                    if (item.Franchise == null) item.Franchise = "";
                    if (item.Comment == null) item.Comment = "";

                    var UPD = _context.spr_NetworkName.Where(w => w.Id == item.Id).Single();
                    var UPD_NNP = _context.spr_NetworkName_Period.Where(w => w.spr_NetworkNameId == item.Id && w.period == item.Period).Single();

                    UPD.Associations = item.Associations;
                    UPD.Franchise = item.Franchise;
                    UPD.Comment = item.Comment;
                    UPD.CompanyDescription = item.CompanyDescription;
                    UPD.RegistrationYear = item.RegistrationYear;
                    UPD.Top5Distributors = item.Top5Distributors;
                    UPD.STM_Brands = item.STM_Brands;
                    UPD.OtherInformation = item.OtherInformation;
                    UPD.TopManagerPosition = item.TopManagerPosition;
                    UPD.TopManagerName = item.TopManagerName;
                    UPD.OwnerName = item.OwnerName;
                    UPD.HeadOfficeLegalAddress = item.HeadOfficeLegalAddress;
                    UPD.HeadOfficeActualAddress = item.HeadOfficeActualAddress;
                    UPD.Phone = item.Phone;
                    UPD.Email = item.Email;


                    UPD_NNP.PreferentialRecipesSalesSum = item.PreferentialRecipesSalesSum;
                    UPD_NNP.AverageReceipt = item.AverageReceipt;
                    UPD_NNP.SKUTotalCount = item.SKUTotalCount;
                    UPD_NNP.Rx_Share = item.Rx_Share;
                    UPD_NNP.OTC_Share = item.OTC_Share;
                    UPD_NNP.BAD_Share = item.BAD_Share;
                    UPD_NNP.Other_Share = item.Other_Share;
                    UPD_NNP.STM_Share = item.STM_Share;
                    UPD_NNP.Ecom_Share = item.Ecom_Share;
                    UPD_NNP.TotalSalesSum = item.TotalSalesSum;





                    if (UPD.Value != item.Value)
                    {
                        _context.SaveChanges();
                        _context.spr_NetworkName_Update(UPD.Value, item.Value);
                    }

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
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Network_FromExcel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            string filename = @"\\s-sql2\Upload\Network_" + User.Identity.GetUserId() + ".xlsx";
            try
            {
                var file = uploads.First();
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                file.SaveAs(filename);
                var _context = new GSContext(APP);
                _context.Network_FromExcel(User.Identity.GetUserId());

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null, count = 0, status = "ок", Success = true }
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

        [Authorize(Roles = "GS_View")]
        [HttpPost]
        public ActionResult BookOfChange_Init()
        {
            try
            {
                var _context = new GSContext(APP);

                var formingTransaction = _context.BookOfChangeFormingTransaction.ToList();
                var formingRebranding = _context.BookOfChangeRebranding.ToList();
    
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { 
                        Data = formingTransaction, 
                        Data2 = formingRebranding,
                        count = 1, status = "ок", Success = true 
                    }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult BookOfChange_from_Excel(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    return null;

                var _context = new GSContext(APP);

                var file = uploads.First();
                string filename = @"\\s-sql2\Upload\Книга перемен_" + User.Identity.GetUserId() + ".xlsx";

                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);

                file.SaveAs(filename);

                _context.BookOfChange_from_Excel(filename);

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
        public ActionResult BookOfChange_relodQlik()
        {
            try
            {
                using (var _context = new GSContext(APP))
                {
                    _context.Database.CommandTimeout = 30;
                    _context.Database.ExecuteSqlCommand("exec [dbo].[BookOfChange_relodQlik]");
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
    }

    public class HystoryFilter
    {
        public int top { get; set; }
        public string DataSourceType { get; set; }
        public string Source_client { get; set; }
        public string Comments { get; set; }
        public string DataSource { get; set; }
        public string NetworkName { get; set; }
        public string Spec { get; set; }
        public string text { get; set; }
        public string INN { get; set; }
        public string Address { get; set; }
        public string GSIDs { get; set; }
        public string Ids { get; set; }
        public string PharmacyIDs { get; set; }
        public byte Category { get; set; }
        public int Status { get; set; }
        public void isnull()
        {
            if (Source_client == null)
                Source_client = "";
            if (Comments == null)
                Comments = "";
            if (text == null)
                text = "";
            if (INN == null)
                INN = "";
            if (Address == null)
                Address = "";
            if (GSIDs == null)
                GSIDs = "";
            if (Ids == null)
                Ids = "";
            if (PharmacyIDs == null)
                PharmacyIDs = "";
            if (DataSource == null)
                DataSource = "";
            if (DataSourceType == null)
                DataSourceType = "";
            if (NetworkName == null)
                NetworkName = "";
            if (Spec == null)
                Spec = "";
        }

    }
    public class sprItemUser
    {
        public Guid code { get; set; }
        public string Status { get; set; }
    }
    public class sprItem
    {
        public int code { get; set; }
        public string Status { get; set; }
    }
    public class sprItemLg
    {
        public long code { get; set; }
        public string Status { get; set; }
    }
    public class sprItemst
    {
        public string code { get; set; }
        public string Status { get; set; }
    }
    public class History_init_class
    {
        public object spr_DataSourceType { get; set; }
        public object spr_Spec { get; set; }
        public object spr_DataSource { get; set; }
        // public object spr_NetworkName { get; set; }
        public object spr_Comments { get; set; }
        public object spr_Status { get; set; }
        public object spr_Category { get; set; }
        public object spr_Source_client { get; set; }
        public object spr_Tops { get; set; }
        public object spr_Users { get; set; }
    }
    public class GS_init_class
    {
        public List<string> periodsNames { get; set; }
        public object periods { get; set; }
        public object spr_FormatLayout { get; set; }
        public object spr_PharmacySellingPlaceType { get; set; }
        public object spr_WorkFormat { get; set; }
        public object spr_PointCategory { get; set; }
    }
    public class JsonResult_bricks : JsonResult
    {
        public object L7_list { get; set; }
        public object L6_list { get; set; }
        public object L5_list { get; set; }
        public object L4_list { get; set; }
        public object L3_list { get; set; }
    }
    public class JsonResult_base_address
    {
        public object Data { get; set; }
        public int count { get; set; }
        public int count_to_work { get; set; }
        public int count_not_in_GS { get; set; }
        public int count_in_GS { get; set; }
        public int count_isUse { get; set; }

        public string status { get; set; }
        public bool Success { get; set; }
    }
    public class JsonResult_Organization
    {
        public object Data { get; set; }
        public int count { get; set; }
        public int count_to_work { get; set; }
        public int count_in_GS { get; set; }
        public int count_not_in_GS { get; set; }
        public int count_withNull { get; set; }
        public int count_IsNotCheck { get; set; }
        public int count_IsErrors { get; set; }
        public int count_in_LPU { get; set; }
        public int count_in_DO { get; set; }
        public int count_in_NN { get; set; }
        public string status { get; set; }
        public bool Success { get; set; }
    }
    public class JsonResult
    {
        public object Data { get; set; }
        public object Data2 { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public bool Success { get; set; }
    }
    public class JsonResult_brick
    {
        public object Data { get; set; }
        public object L7_label2 { get; set; }
        public object CityType { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public bool Success { get; set; }
    }
    public class FilterBricks
    {
        public string ids { get; set; }
        public string common { get; set; }
        public string post_index { get; set; }
    }
    public class FilterLicenses
    {
        public string common { get; set; }
        public string activity_type { get; set; }
        public string adress { get; set; }
        public string works { get; set; }
        public DateTime date { get; set; }
        public bool isNew { get; set; }
        public bool withAdress { get; set; }
    }
    public class FilterReestr
    {
        public string common { get; set; }
        public string adress { get; set; }
        public string BrickId { get; set; }
        public string NetworkName { get; set; }
        public string OperationMode { get; set; }
        public string PharmacyBrand { get; set; }
        public bool isNotChecked { get; set; }
        public bool isNew { get; set; }
        public bool isCloseOFD { get; set; }
        public bool isCloseAlphaBit { get; set; }
        public bool isDoubleA { get; set; }
        public bool isLicExists { get; set; }
        public bool isCall { get; set; }
        public string IDS { get; set; }
        public string PHids { get; set; }
        public bool isDateAddLic { get; set; }
        public DateTime? dt { get; set; }
        public bool BrickError { get; set; }
    }
    public class filter_base_address
    {
        public string common { get; set; }
        public bool toWork { get; set; }
        public byte in_GS { get; set; }
        public bool isUse { get; set; }
    }
    public class Filter_Organization
    {
        public string ids { get; set; }
        public string common { get; set; }
        public string inn { get; set; }
        public bool toWork { get; set; }
        public bool withNull { get; set; }
        public bool IsNotCheck { get; set; }
        public bool IsErrors { get; set; }
        public byte in_GS { get; set; }
    }
    public class SparkInn
    {
        public string inn { get; set; }
        public int RN { get; set; }
    }
}