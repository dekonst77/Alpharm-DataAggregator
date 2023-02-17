using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DataReport;
using DataAggregator.Web.App_Start;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers
{
    public class GlobalController : BaseController
    {
        //private DataReportContext _context;

        //protected override void Initialize(RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);
        //    _context = new DataReportContext(APP);
        //}

        //~GlobalController()
        //{
        //    _context.Dispose();
        //}

        [Authorize(Roles = "Admin")]
        public ActionResult query_Init()
        {
            try
            {
                using (var context = new DataReportContext(APP))
                {
                    ViewData["Area"] = context.WebAggReports.Select(s => s.Area).OrderBy(o => o).Distinct().ToList();

                    var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = Data
                    };
                    return jsonNetResult;
                }
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
        [Authorize(Roles = "Admin")]
        public ActionResult query_search()
        {
            try
            {
                using (var context = new DataReportContext(APP))
                {
                    ViewData["query"] = context.WebAggReports.OrderBy(o => o.Name).ToList();

                    var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = Data
                    };
                    return jsonNetResult;
                }
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
        [Authorize(Roles = "Admin")]
        public ActionResult query_save(ICollection<DataAggregator.Domain.Model.DataReport.WebAggReports> array)
        {
            try
            {
                using (var context = new DataReportContext(APP))
                {
                    if (array != null)
                        foreach (var item in array)
                        {
                            if (item.Id > 0)
                            {
                                var upd = context.WebAggReports.Where(w => w.Id == item.Id).Single();
                                upd.Name = item.Name;
                                upd.Server = item.Server;
                                upd.Area = item.Area;
                                upd.Roles = item.Roles;
                                upd.Query = item.Query;
                                upd.Filters = item.Filters;
                            }
                            else
                            {
                                context.WebAggReports.Add(
                                    new Domain.Model.DataReport.WebAggReports()
                                    {
                                        Name = item.Name,
                                        Server = item.Server,
                                        Area = item.Area,
                                        Roles = item.Roles,
                                        Query = item.Query,
                                        Filters = item.Filters
                                    });
                            }
                        }
                    context.SaveChanges();
                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                    };
                    return jsonNetResult;
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }




        public string GetValueString(string param, string value)
        {
            string ret = "";
            //{"model":{"name":"gs_trigger_log","purchase_id":"5640886"}}
            int p1 = param.IndexOf("\"" + value + "\":");
            if (p1 > 0)
            {
                p1 += value.Length + 4;
                int p2 = param.IndexOf("\"", p1);
                if (p2 > 0)
                {
                    ret = param.Substring(p1, p2 - p1);
                }
            }

            return ret;
        }
        [HttpPost]
        public ActionResult GetReportList()
        {
            try
            {
                using (var context = new DataReportContext(APP))
                {
                    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    string[] Roles = userManager.GetRoles(User.Identity.GetUserId()).ToArray();
                    var GZ_report = context.WebAggReports.Where(w => w.Area == "GovernmentPurchases" && Roles.Contains(w.Roles)).ToList();
                    ViewBag.GZ_report = GZ_report;
                    var GS_report = context.WebAggReports.Where(w => w.Area == "geomarketing" && Roles.Contains(w.Roles)).ToList();
                    ViewBag.GS_report = GS_report;
                    var Prov_report = context.WebAggReports.Where(w => w.Area == "Prov" && Roles.Contains(w.Roles)).ToList();
                    ViewBag.Prov_report = Prov_report;
                    var Distr_report = context.WebAggReports.Where(w => w.Area == "Distr" && Roles.Contains(w.Roles)).ToList();
                    ViewBag.Distr_report = Distr_report;

                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = new JsonResult() { Data = ViewBag }
                    };
                    return jsonNetResult;
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        private void GetWebReport(int id, bool isInit, List<cField> FLD, List<cField> Filters)
        {
            using (var context = new DataReportContext(APP))
            {
                WebAggReports report;
                using (var _context = new DataReportContext(APP))
                {
                    report = _context.WebAggReports.Where(w => w.Id == id).Single();
                }

                ReportsLogView isRunningProcess = null;
                var fltTemp = "";
                if (Filters != null && Filters.Count > 0)
                {
                    foreach (var flt in Filters)
                    {
                        switch (flt.sType)
                        {
                            case "date":
                                flt.Value = Convert.ToString(flt.Value).Substring(0, 10);
                                break;
                        }
                    }
                    fltTemp = String.Join("", Filters.ToArray().Select(x => JsonConvert.SerializeObject(x)));
                }

                if (!string.IsNullOrEmpty(fltTemp))
                {
                    isRunningProcess = context.ReportsLogView.FirstOrDefault(x => x.ReportId == id && x.Filters == fltTemp && x.DateStart != null && x.DateEnd == null && x.StatusId == 0);
                }

                if (isRunningProcess != null)
                {
                    var stat = context.ReportsLog.Where(x => x.ReportId == id && x.DateStart != null && x.DateEnd != null && x.StatusId == 1)
                        .Select(x => SqlFunctions.DateDiff("second", x.DateStart.Value, x.DateEnd.Value) / 60.0)
                        .Average();

                    var info = new StringBuilder();
                    info.AppendFormat("Пользователь {0} уже запустил отчет \"{1}\" в {2} мск.<br/>" +
                        "Среднее время выполнения данного отчета {3} минут(ы).<br/>" +
                        "Попробуйте сформировать позже!",
                        isRunningProcess.FullName,
                        report.Name,
                        (isRunningProcess.DateStart != null ? isRunningProcess.DateStart.Value.ToString("dd.MM.yyyy HH:mm:ss") : DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")),
                        (stat == null || (stat.HasValue && stat.Value == 0) ? "5" : Math.Round(stat.Value, 2).ToString()));
                    ViewBag.RunningProcess = info.ToString();
                }
                else
                {
                    ViewBag.RunningProcess = null;
                    string query = report.Query;
                    if (isInit)
                    {
                        if (!string.IsNullOrEmpty(report.Filters))
                        {
                            foreach (string item in report.Filters.Split(','))
                            {
                                switch (item.Split('-')[2])
                                {
                                    case "bool":
                                        Filters.Add(new cField() { Name = item.Split('-')[0], DisplayName = item.Split('-')[1], IsEdit = true, IsKey = false, sType = item.Split('-')[2], Value = false });
                                        break;
                                    case "date":
                                        Filters.Add(new cField() { Name = item.Split('-')[0], DisplayName = item.Split('-')[1], IsEdit = true, IsKey = false, sType = item.Split('-')[2], Value = DateTime.Now });
                                        break;
                                    case "string":
                                        Filters.Add(new cField() { Name = item.Split('-')[0], DisplayName = item.Split('-')[1], IsEdit = true, IsKey = false, sType = item.Split('-')[2], Value = "" });
                                        break;
                                    case "int":
                                        Filters.Add(new cField() { Name = item.Split('-')[0], DisplayName = item.Split('-')[1], IsEdit = true, IsKey = false, sType = item.Split('-')[2], Value = 0 });
                                        break;
                                    case "double":
                                        Filters.Add(new cField() { Name = item.Split('-')[0], DisplayName = item.Split('-')[1], IsEdit = true, IsKey = false, sType = item.Split('-')[2], Value = 0 });
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Filters != null && Filters.Count > 0)
                        {
                            foreach (var flt in Filters)
                            {
                                switch (flt.sType)
                                {
                                    case "date":
                                        query = query.Replace(flt.Name, Convert.ToString(flt.Value));
                                        break;
                                    case "string":
                                        query = query.Replace(flt.Name, Convert.ToString(flt.Value));
                                        break;
                                    case "int":
                                        query = query.Replace(flt.Name, Convert.ToString(flt.Value));
                                        break;
                                    case "bool":
                                        query = query.Replace(flt.Name, Convert.ToString(flt.Value));
                                        break;
                                    case "double":
                                        query = query.Replace(flt.Name, Convert.ToString(flt.Value));
                                        break;
                                }
                            }
                        }

                        DataTable tbl = null;
                        var userId = User.Identity.GetUserId() != null ? new Guid(User.Identity.GetUserId()) : Guid.Empty;
                        var log = context.LogStart(report, Filters, userId);
                        try
                        {
                            using (var command = new SqlCommand())
                            {
                                command.Connection = new SqlConnection("Persist Security Info=true;Server=" + report.Server + ";Database=tempdb;Integrated Security=SSPI;APP=" + APP);

                                if (command.Connection.State == ConnectionState.Closed)
                                    command.Connection.Open();

                                SqlTransaction sqlTran = command.Connection.BeginTransaction(IsolationLevel.Snapshot);
                                command.CommandTimeout = 0;
                                command.CommandText = query;
                                command.Transaction = sqlTran;
                                tbl = new DataTable("tbl");
                                try
                                {
                                    tbl.Load(command.ExecuteReader());
                                    if (command.Transaction != null)
                                        command.Transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        if (command.Transaction != null)
                                            command.Transaction.Rollback();
                                    }
                                    catch
                                    {
                                    }
                                    throw ex;
                                }
                            }
                            log.Note = tbl.Rows.Count.ToString();
                            context.LogEnd(log, 1);
                        }
                        catch (Exception ex)
                        {
                            var errorMsg = ex.Message;
                            while (ex.InnerException != null)
                            {
                                ex = ex.InnerException;
                                errorMsg += ex.Message;
                            }
                            log.Note = errorMsg;
                            context.LogEnd(log, 2);
                            throw ex;
                        }

                        if (FLD != null && tbl != null)
                        {
                            string sType = "";
                            foreach (System.Data.DataColumn c in tbl.Columns)
                            {
                                sType = "string";
                                if (c.DataType.Name.Contains("Int"))
                                    sType = "int";
                                if (c.DataType.Name.Contains("Decimal"))
                                    sType = "double";
                                if (c.DataType.Name.Contains("DateTime"))
                                    sType = "datetime";
                                FLD.Add(new cField() { Name = c.ColumnName, DisplayName = c.ColumnName, IsEdit = false, IsKey = false, sType = sType });
                            }
                        }
                        if (tbl != null)
                        {
                            ViewBag.Result = tbl;
                            ViewBag.Data = GetWebReportJson(tbl);
                        }
                        ViewBag.TypeData = "string";
                    }
                }
                ViewBag.Fields = FLD;
                ViewBag.Name = report.Name;
                ViewBag.title = report.Name;
            }
        }
        public static string GetWebReportJson(System.Data.DataTable table)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            jsSerializer.MaxJsonLength = int.MaxValue;
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (System.Data.DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    if (col.DataType.Name.Contains("DateTime") && row[col] != null && !(row[col] is System.DBNull))
                    {
                        childRow.Add(col.ColumnName, ((DateTime)row[col]).ToString("yyyy-MM-ddTHH:mm:ss"));
                    }
                    else
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        [HttpPost]
        public ActionResult Init(string name, string param)
        {
            string title = "";
            try
            {
                List<cField> FLD = new List<cField>();
                List<cField> Filters = new List<cField>();
                List<cCommand> CMD = new List<cCommand>();
                List<cSPR> SPR = new List<cSPR>();
                bool Search_now = true;
                switch (name)
                {
                    case "WebReport":
                        GetWebReport(Convert.ToInt32(GetValueString(param, "id")), true, FLD, Filters);
                        Search_now = false;
                        break;
                    case "ClassBlocked":
                        title = "Блокированные позиции";
                        ViewBag.Name = title;
                        FLD.Add(new cField() { Name = "Id", DisplayName = "Id", IsEdit = false, IsKey = true, sType = "int" });
                        FLD.Add(new cField() { Name = "ClassifierId", DisplayName = "ClassifierId", IsEdit = false, IsKey = false, sType = "int" });
                        FLD.Add(new cField() { Name = "DrugId", DisplayName = "DrugId", IsEdit = false, IsKey = false, sType = "int" });
                        FLD.Add(new cField() { Name = "TradeName", DisplayName = "TradeName", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "FV", DisplayName = "FV", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "OwnerTradeMarkId", DisplayName = "OwnerTradeMarkId", IsEdit = false, IsKey = false, sType = "int" });
                        FLD.Add(new cField() { Name = "OwnerTradeMark", DisplayName = "OwnerTradeMark", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "PackerId", DisplayName = "PackerId", IsEdit = false, IsKey = false, sType = "int" });
                        FLD.Add(new cField() { Name = "Packer", DisplayName = "Packer", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "Comment", DisplayName = "Ком Пров", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "Used", DisplayName = "Используется", IsEdit = false, IsKey = false, sType = "bool" });
                        FLD.Add(new cField() { Name = "Comment2", DisplayName = "Ком Блокир", IsEdit = true, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "Data_setBlock", DisplayName = "блок отмечен", IsEdit = false, IsKey = false, sType = "date" });
                        FLD.Add(new cField() { Name = "Data_Block", DisplayName = "Заблокирован с", IsEdit = true, IsKey = false, sType = "date" });
                        FLD.Add(new cField() { Name = "Data_UnBlock", DisplayName = "Разблокирован с", IsEdit = true, IsKey = false, sType = "date" });
                        FLD.Add(new cField() { Name = "kofPriceGZotkl", DisplayName = "kofPriceGZotkl", IsEdit = true, IsKey = false, sType = "double" });


                        Filters.Add(new cField() { Name = "DateBlock", DisplayName = "Есть даты блокировок", IsEdit = true, IsKey = false, sType = "bool", Value = false });
                        Filters.Add(new cField() { Name = "Used", DisplayName = "Заблокированные", IsEdit = true, IsKey = false, sType = "bool", Value = true });
                        Filters.Add(new cField() { Name = "inGZ", DisplayName = "В госзакупках", IsEdit = true, IsKey = false, sType = "bool", Value = true });

                        CMD.Add(new cCommand() { Name = "report", DisplayName = "Отчёт", command = "/#/Global?name=WebReport&id=9", typec = "href" });
                        Search_now = true;
                        break;
                    case "GS_Pharmacy":
                        title = "Точки PharmacyID";
                        ViewBag.Name = title;
                        FLD.Add(new cField() { Name = "PharmacyId", DisplayName = "PharmacyId", IsEdit = false, IsKey = true, sType = "int" });
                        FLD.Add(new cField() { Name = "date_add", DisplayName = "Дата Появления", IsEdit = false, IsKey = false, sType = "date" });
                        FLD.Add(new cField() { Name = "GSId_first", DisplayName = "Первый GSId", IsEdit = false, IsKey = false, sType = "int" });
                        FLD.Add(new cField() { Name = "koor_широта", DisplayName = "широта(lat)", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "koor_долгота", DisplayName = "долгота(lon)", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "Address", DisplayName = "Address", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "BricksId", DisplayName = "BricksId", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "fias_id_manual", DisplayName = "fias_id_manual", IsEdit = true, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "fias_code_manual", DisplayName = "fias_code_manual", IsEdit = true, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "geo_lat_manual", DisplayName = "geo_lat_manual", IsEdit = true, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "geo_lon_manual", DisplayName = "geo_lon_manual", IsEdit = true, IsKey = false, sType = "string" });

                        Filters.Add(new cField() { Name = "NotKoor", DisplayName = "нет координат", IsEdit = true, IsKey = false, sType = "bool", Value = true });

                        Search_now = true;
                        break;
                    case "gs_trigger_log":
                        title = "лог изменений ГЗ";
                        ViewBag.Name = title;
                        FLD.Add(new cField() { Name = "Purchase_Id", DisplayName = "Purchase_Id", IsEdit = false, IsKey = false, sType = "int" });
                        FLD.Add(new cField() { Name = "Lot_Id", DisplayName = "Lot_Id", IsEdit = false, IsKey = false, sType = "int" });
                        FLD.Add(new cField() { Name = "Contract_Id", DisplayName = "Contract_Id", IsEdit = false, IsKey = false, sType = "int" });

                        FLD.Add(new cField() { Name = "Who", DisplayName = "Кто", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "What", DisplayName = "Что", IsEdit = false, IsKey = false, sType = "string" });

                        FLD.Add(new cField() { Name = "When", DisplayName = "Когда", IsEdit = false, IsKey = false, sType = "datetime" });

                        Filters.Add(new cField() { Name = "Purchase_Id", DisplayName = "Purchase_Id", IsEdit = true, IsKey = false, sType = "string", Value = GetValueString(param, "purchase_id") });
                        Filters.Add(new cField() { Name = "Lot_Id", DisplayName = "Lot_Id", IsEdit = true, IsKey = false, sType = "string", Value = GetValueString(param, "lot_id") });
                        Filters.Add(new cField() { Name = "Contract_Id", DisplayName = "Contract_Id", IsEdit = true, IsKey = false, sType = "string", Value = GetValueString(param, "contract_id") });
                        Search_now = true;
                        break;
                    case "dictMethod":
                        title = "Сопоставление Методов";
                        ViewBag.Name = title;
                        FLD.Add(new cField() { Name = "Id", DisplayName = "Id", IsEdit = false, IsKey = true, sType = "int" });
                        FLD.Add(new cField() { Name = "Name", DisplayName = "Name", IsEdit = false, IsKey = false, sType = "string" });
                        FLD.Add(new cField() { Name = "MethodId", DisplayName = "MethodId", IsEdit = true, IsKey = false, sType = "SPR", SPR = "Method" });

                        var _context_Method = new GovernmentPurchasesContext(APP);
                        var Method = _context_Method.Method.Select(s => s).OrderBy(o => o.Name);
                        SPR.Add(new cSPR()
                        {
                            Name = "Method",
                            Data = Method.Select(s => new cSPRItem() { Id = s.Id, Value = s.Name }).ToList()
                        });
                        Search_now = true;
                        break;
                    case "AutoCorrectAmountInfo":
                        title = "AutoCorrectAmountInfo";
                        ViewBag.Name = title;
                        FLD.Add(new cField() { Name = "Unit", DisplayName = "Unit", IsEdit = true, IsKey = true, sType = "string" });
                        FLD.Add(new cField() { Name = "Type", DisplayName = "Type", IsEdit = true, IsKey = false, sType = "SPR", SPR = "UnitType" });

                        var _context_AutoCorrectAmountInfo = new GovernmentPurchasesContext(APP);
                        var UnitType = _context_AutoCorrectAmountInfo.UnitType.Select(s => s).OrderBy(o => o.Id);
                        SPR.Add(new cSPR()
                        {
                            Name = "UnitType",
                            Data = UnitType.Select(s => new cSPRItem() { Id = s.Id, Value = s.Value }).ToList()
                        });

                        CMD.Add(new cCommand() { Name = "AddNew", DisplayName = "Новая строка", command = "AutoCorrectAmountInfo", typec = "AddNew" });
                        Search_now = true;
                        break;
                    case "AutoCorrectDosageRecount":
                        title = "AutoCorrectDosageRecount";
                        ViewBag.Name = title;
                        var _context_w1 = new GovernmentPurchasesContext(APP);
                        _context_w1.Database.ExecuteSqlCommand(@"
                            exec DrugClassifier.[GoodsClassifier].[Goods_create_GoodsDescription_id]

                            insert into [dbo].[AutoCorrectDosageRecount]([DosageGroupId],[FormProductId],[ConsumerPackingCount],CoeffConsumerPackingCount)
                            select ev.DosageGroupId,IIF(ev.DrugId>0,ev.FormProductId,-1*DD.id),
                            IIF(ev.DrugId>0, ev.ConsumerPackingCount,DD.ConsumerPackingCount),IIF(ev.DrugId>0, ev.ConsumerPackingCount,DD.ConsumerPackingCount)
                            from DrugClassifier.Classifier.ExternalView_FULL ev
                            left join DrugClassifier.[GoodsClassifier].[GoodsDescription_for_Hydra] DD on DD.Value=ev.DrugDescription and ev.GoodsId>0
                            left join GovernmentPurchases.dbo.AutoCorrectDosageRecount as dr 
                            on (ev.DosageGroupId = dr.DosageGroupId or dr.DosageGroupId is null and ev.DosageGroupId is null) 
                            and IIF(ev.DrugId>0,ev.FormProductId,-1*DD.id) = dr.FormProductId and IIF(ev.DrugId>0, ev.ConsumerPackingCount,DD.ConsumerPackingCount) = dr.ConsumerPackingCount
                            where dr.Id is null and IIF(ev.DrugId>0, ev.ConsumerPackingCount,DD.ConsumerPackingCount)>0
                            group by ev.DosageGroupId,IIF(ev.DrugId>0,ev.FormProductId,-1*DD.id),
                            IIF(ev.DrugId>0, ev.ConsumerPackingCount,DD.ConsumerPackingCount),IIF(ev.DrugId>0, ev.ConsumerPackingCount,DD.ConsumerPackingCount)
                        ");

                        FLD.Add(new cField() { Name = "Id", DisplayName = "Id", IsEdit = false, IsKey = true, sType = "int" });
                        FLD.Add(new cField() { Name = "DosageGroupId", DisplayName = "DosageGroupId", IsEdit = false, IsKey = false, sType = "SPR", SPR = "DosageGroups" });
                        FLD.Add(new cField() { Name = "FormProductId", DisplayName = "FormProductId", IsEdit = false, IsKey = false, sType = "SPR", SPR = "FormProducts" });
                        FLD.Add(new cField() { Name = "ConsumerPackingCount", DisplayName = "ConsumerPackingCount", IsEdit = false, IsKey = false, sType = "int" });

                        FLD.Add(new cField() { Name = "CoeffConsumerPackingCount", DisplayName = "CoeffConsumerPackingCount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffMgAmount", DisplayName = "CoeffMgAmount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffMlAmount", DisplayName = "CoeffMlAmount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffGAmount", DisplayName = "CoeffGAmount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffDosAmount", DisplayName = "CoeffDosAmount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffMeAmount", DisplayName = "CoeffMeAmount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffKgAmount", DisplayName = "CoeffKgAmount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffLAmount", DisplayName = "CoeffLAmount", IsEdit = true, IsKey = false, sType = "double" });
                        FLD.Add(new cField() { Name = "CoeffM3Amount", DisplayName = "CoeffM3Amount", IsEdit = true, IsKey = false, sType = "double" });

                        var _context_AutoCorrectDosageRecount = new DrugClassifierContext(APP);
                        var FormProducts = _context_AutoCorrectDosageRecount.Database.SqlQuery<cSPRItem>(@"
                            select Id,Value from Classifier.FormProduct
                            union
                            select -1*Id as Id,Value from [GoodsClassifier].[GoodsDescription_for_Hydra]
                            ");
                        //.FormProducts.Select(s => s).OrderBy(o => o.Id);
                        var DosageGroups = _context_AutoCorrectDosageRecount.Database.SqlQuery<cSPRItem>(@"
                            select Id,Description Value from Classifier.DosageGroup
                            --union
                            --select -1*Id as Id,Value from [GoodsClassifier].[GoodsDescription_for_Hydra]
                            ");
                        //DosageGroups.Select(s => s).OrderBy(o => o.Id);

                        SPR.Add(new cSPR()
                        {
                            Name = "FormProducts",
                            Data = FormProducts.ToList()
                        });

                        SPR.Add(new cSPR()
                        {
                            Name = "DosageGroups",
                            Data = DosageGroups.ToList()
                        });
                        Search_now = true;
                        break;
                }

                ViewBag.Fields = FLD;
                ViewBag.Filters = Filters;
                ViewBag.CMD = CMD;
                ViewBag.SPR = SPR;
                ViewBag.Search_now = Search_now;
                if (title != "")
                    ViewBag.title = title;

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public FileResult LoadExcel(string name, string param, List<cField> Filters)
        {
            Load(name, param, Filters);
            if (ViewBag.Result != null)
            {
                Excel.Excel excel = new Excel.Excel();
                excel.Create();
                excel.InsertDataTable("Отчёт", 1, 1, ViewBag.Result, true, true, null);
                byte[] bb = excel.SaveAsByte();
                return File(bb, "application/vnd.ms-excel", "Отчёт.xlsx");
            }
            else {
                return null;
            }
        }
        [HttpPost]
        public ActionResult Load(string name, string param, List<cField> Filters)
        {
            try
            {
                string sql = "";
                switch (name)
                {
                    case "WebReport":
                        List<cField> FLD = new List<cField>();
                        GetWebReport(Convert.ToInt32(GetValueString(param, "id")), false, FLD, Filters);
                        ViewBag.Fields = FLD;
                        break;
                    case "ClassBlocked":
                        if (!User.IsInRole("SPharmacist") && !User.IsInRole("GManager"))
                        {
                            return Forbidden();
                        }
                        sql = "select * from [DrugClassifier].[Classifier].[ProductionInfoView] where 0=0 ";

                        foreach (var filter in Filters)
                        {
                            switch (filter.Name)
                            {
                                case "DateBlock":
                                    if ((bool)filter.Value == true)
                                    {
                                        sql += " and Data_Block is not null";
                                    }
                                    break;
                                case "inGZ":
                                    if ((bool)filter.Value == true)
                                    {
                                        sql += " and id in (select ProductionInfoId from [GovernmentPurchases].[dbo].[ClassifierInBD] where ProductionInfoId>0)";
                                    }
                                    break;
                                case "Used":
                                    if ((bool)filter.Value == true)
                                    {
                                        sql += " and Used=0";
                                    }
                                    else
                                    {
                                        sql += " and Used=1";
                                    }
                                    break;
                            }
                        }
                        var _context = new DrugClassifierContext(APP);
                        _context.Database.CommandTimeout = 0;
                        sql += " order by id";
                        var ret = _context.Database.SqlQuery<DataAggregator.Domain.Model.DrugClassifier.Classifier.ProductionInfoView>(sql).ToList();
                        ViewBag.Result = ret;
                        ViewBag.Data = ret;
                        break;
                    case "dictMethod":
                        var _context_Method = new GovernmentPurchasesLoaderContext(APP);
                        _context_Method.Database.CommandTimeout = 0;
                        var ret_dictMethod = _context_Method.MethodDictionary.Select(s => s).ToList();
                        ViewBag.Result = ret_dictMethod;
                        ViewBag.Data = ret_dictMethod;
                        break;
                    case "AutoCorrectAmountInfo":
                        //sql = "select * from [GovernmentPurchases].[dbo].[AutoCorrectAmountInfo]";
                        var _context_AutoCorrectAmountInfo = new GovernmentPurchasesContext(APP);
                        _context_AutoCorrectAmountInfo.Database.ExecuteSqlCommand(@"  
select unit into #new from [dbo].[PurchaseObjectReady] group by unit
insert into [dbo].[AutoCorrectAmountInfo]([Unit],[Action],[Type])
select unit,'нет',0 from #new where unit not in (select unit from [dbo].[AutoCorrectAmountInfo])");
                        _context_AutoCorrectAmountInfo.Database.ExecuteSqlCommand(@"  
select unit into #new from [dbo].[ContractObjectReady] group by unit
insert into [dbo].[AutoCorrectAmountInfo]([Unit],[Action],[Type])
select unit,'нет',0 from #new where unit not in (select unit from [dbo].[AutoCorrectAmountInfo])");
                        _context_AutoCorrectAmountInfo.Database.CommandTimeout = 0;
                        var ret_AutoCorrectAmountInfo = _context_AutoCorrectAmountInfo.AutoCorrectAmountInfo.Select(s => s).ToList();
                        ViewBag.Result = ret_AutoCorrectAmountInfo;
                        ViewBag.Data = ret_AutoCorrectAmountInfo;
                        break;
                    case "AutoCorrectDosageRecount":
                        //sql = "select * from [GovernmentPurchases].[dbo].[AutoCorrectAmountInfo]";
                        var _context_AutoCorrectDosageRecount = new GovernmentPurchasesContext(APP);
                        _context_AutoCorrectDosageRecount.Database.CommandTimeout = 0;
                        var ret_AutoCorrectDosageRecount = _context_AutoCorrectDosageRecount.AutoCorrectDosageRecount.Select(s => s).ToList();
                        ViewBag.Result = ret_AutoCorrectDosageRecount;
                        ViewBag.Data = ret_AutoCorrectDosageRecount;
                        break;
                    case "gs_trigger_log":
                        sql = "select * from [logs].[TriggerLog] where 0=0";
                        foreach (var filter in Filters)
                        {
                            if (!string.IsNullOrEmpty((string)filter.Value))
                                sql += " and " + filter.Name + " = " + (string)filter.Value;
                        }
                        var _context_gs_trigger_log = new GovernmentPurchasesContext(APP);
                        _context_gs_trigger_log.Database.CommandTimeout = 0;
                        var ret_gs_trigger_log = _context_gs_trigger_log.Database.SqlQuery<DataAggregator.Domain.Model.GovernmentPurchases.TriggerLog>(sql).ToList();

                        ViewBag.Result = ret_gs_trigger_log;
                        ViewBag.Data = ret_gs_trigger_log;
                        break;
                    case "GS_Pharmacy":
                        sql = "select * from [GS].[dbo].[Pharmacy] where 0=0 ";
                        var _context_GS = new GSContext(APP);
                        _context_GS.Database.CommandTimeout = 0;
                        foreach (var filter in Filters)
                        {
                            switch (filter.Name)
                            {
                                case "NotKoor":
                                    if ((bool)filter.Value == true)
                                    {
                                        sql += @" and [PharmacyId] in(
  select p.PharmacyId
  FROM
[out].[Pharmacy] p
  INNER JOIN dbo.Pharmacy spr_p ON spr_p.PharmacyId = p.PharmacyId
  inner join[dbo].[Address] ADDR on spr_p.Address= ADDR.Original
where PERIOD_KEY>=201901 and (koor_широта is null or koor_широта = 0)
)";
                                    }
                                    break;
                            }
                        }
                        if (sql != "")
                        {
                            var ret_2 = _context_GS.Database.SqlQuery<DataAggregator.Domain.Model.GS.Pharmacy>(sql).ToList();

                            ViewBag.Result = ret_2;
                            ViewBag.Data = ret_2;
                        }
                        break;
                }
                var jsonResult = new JsonResult() { Data = ViewBag };
                jsonResult.MaxJsonLength = int.MaxValue;
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = jsonResult
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SaveAutoCorrectAmountInfo(List<DataAggregator.Domain.Model.GovernmentPurchases.AutoCorrectAmountInfo> data)
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);
                foreach (var item in data)
                {
                    var upd = _context.AutoCorrectAmountInfo.Where(w => w.Unit == item.Unit).Single();
                    if (upd != null)
                    {
                        upd.Type = item.Type;
                    }
                    else
                    {
                        _context.AutoCorrectAmountInfo.Add(new Domain.Model.GovernmentPurchases.AutoCorrectAmountInfo()
                        {
                            Type = item.Type,
                            Unit = item.Unit
                        });
                    }
                }
                _context.SaveChanges();
                ViewBag.Success = true;


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult SavedictMethod(List<DataAggregator.Domain.Model.GovernmentPurchasesLoader.MethodDictionary> data)
        {
            try
            {
                var _context = new GovernmentPurchasesLoaderContext(APP);
                foreach (var item in data)
                {
                    var upd = _context.MethodDictionary.Where(w => w.Id == item.Id).Single();
                    upd.MethodId = item.MethodId;
                }
                _context.SaveChanges();
                ViewBag.Success = true;


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult SaveAutoCorrectDosageRecount(List<DataAggregator.Domain.Model.GovernmentPurchases.AutoCorrectDosageRecount> data)
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);
                foreach (var item in data)
                {
                    var upd = _context.AutoCorrectDosageRecount.Where(w => w.Id == item.Id).FirstOrDefault();
                    if (upd != null)
                    {
                        upd.CoeffConsumerPackingCount = item.CoeffConsumerPackingCount;
                        upd.CoeffDosAmount = item.CoeffDosAmount;
                        upd.CoeffGAmount = item.CoeffGAmount;
                        upd.CoeffMeAmount = item.CoeffMeAmount;
                        upd.CoeffMgAmount = item.CoeffMgAmount;
                        upd.CoeffMlAmount = item.CoeffMlAmount;

                        upd.CoeffKgAmount = item.CoeffKgAmount;
                        upd.CoeffLAmount = item.CoeffLAmount;
                        upd.CoeffM3Amount = item.CoeffM3Amount;
                    }
                    /* else
                     {
                         _context.AutoCorrectAmountInfo.Add(new Domain.Model.GovernmentPurchases.AutoCorrectAmountInfo()
                         {
                             Type = item.Type,
                             Unit = item.Unit
                         });
                     }*/
                }
                _context.SaveChanges();
                ViewBag.Success = true;


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult SaveClassBlocked(List<DataAggregator.Domain.Model.DrugClassifier.Classifier.ProductionInfoView> data)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                foreach (var item in data)
                {
                    var upd = _context.ProductionInfo.Where(w => w.Id == item.Id).Single();
                    upd.Comment2 = item.Comment2;
                    upd.Data_Block = item.Data_Block;
                    upd.Data_UnBlock = item.Data_UnBlock;
                    upd.kofPriceGZotkl = item.kofPriceGZotkl;
                }
                _context.SaveChanges();
                ViewBag.Success = true;


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult SaveGS_Pharmacy(List<DataAggregator.Domain.Model.GS.Pharmacy> data)
        {
            try
            {
                var _context = new GSContext(APP);
                foreach (var item in data)
                {
                    var upd = _context.Pharmacy.Where(w => w.PharmacyId == item.PharmacyId).Single();
                    upd.fias_code_manual = item.fias_code_manual;
                    upd.fias_id_manual = item.fias_id_manual;
                    upd.geo_lat_manual = item.geo_lat_manual;
                    upd.geo_lon_manual = item.geo_lon_manual;
                }
                _context.SaveChanges();
                ViewBag.Success = true;


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult GCommand(string name, string action)
        {
            try
            {
                switch (name)
                {
                    case "ClassBlocked":
                        if (action == "reportInGZ")
                        {
                            var _context = new DrugClassifierContext(APP);
                        }
                        break;
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        public bool IsEnable(string Server, string Name)
        {
            return true;
        }
        [HttpPost]
        public ActionResult Job(string Server, string Name, bool Run)
        {
            if (!IsEnable(Server, Name))
                return null;
            try
            {
                string strconnection = "Persist Security Info=true;Server=" + Server + ";Database=ControlALG;Integrated Security=SSPI;APP=" + APP;
                DbContext context = new DbContext(strconnection);
                context.Database.Log = (query) => Debug.Write(query);

                ViewBag.Status = DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(context, Name,
                    Run == true ? Domain.Model.ControlALG.ControlALG.JobStartAction.start : Domain.Model.ControlALG.ControlALG.JobStartAction.info
                    ).Replace("\r\n", @"<br />");


                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ViewBag }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
       

    }
}