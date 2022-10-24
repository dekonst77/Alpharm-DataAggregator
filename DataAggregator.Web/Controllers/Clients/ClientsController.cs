using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Clients
{
    [Authorize(Roles = "Clients")]
    public class ClientsController : BaseController
    {
        public ActionResult CompaniesInit()
        {
            return null;
        }
        [HttpPost]
        [Authorize(Roles = "Clients_Manager")]
        public ActionResult CompaniesGet(FilterCompany filter)
        {
            try
            {
                var _context = new DataReportContext(APP);
                var Companies = _context.Companies.Select(s => s);
                if (filter.id > 0)
                {
                    Companies = Companies.Where(w=>w.Id==(int)filter.id);
                }
                if (!string.IsNullOrEmpty(filter.common))
                {
                    Companies = Companies.Where(w => w.Value.Contains(filter.common));
                }
                //
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Companies.OrderBy(o => o.Value).ToList(), count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Clients_Manager")]
        public ActionResult Companies_Save(ICollection<DataAggregator.Domain.Model.DataReport.Companies> array)
        {
            try
            {
                var _context = new DataReportContext(APP);
                foreach (var item in array)
                {
                    if (item.Id > 0)
                    {
                        var upd = _context.Companies.Where(w => w.Id == item.Id).Single();
                        upd.Value = item.Value;
                    }
                    else
                    {
                        _context.Companies.Add(new Domain.Model.DataReport.Companies { Value = item.Value });
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
                return BadRequest(e.Message);
            }
        }
        public ActionResult WorkerGet(int CompanyId)
        {
            try
            {
                var _context = new DataReportContext(APP);
                var Worker = _context.Worker.Where(w=>w.CompanyId== CompanyId).Select(s => s).OrderBy(o => o.Name);
                //
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Worker, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Worker_Save(ICollection<DataAggregator.Domain.Model.DataReport.Worker> array)
        {
            try
            {
                var _context = new DataReportContext(APP);
                foreach (var item in array)
                {
                    if (item.Id > 0)
                    {
                        var upd = _context.Worker.Where(w => w.Id == item.Id).Single();
                        upd.Email = item.Email;
                        upd.Name = item.Name;
                        upd.CompanyId = item.CompanyId;
                    }
                    else
                    {
                        _context.Worker.Add(new Domain.Model.DataReport.Worker { Email = item.Email,CompanyId=item.CompanyId,Name=item.Name });
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
                return BadRequest(e.Message);
            }
        }

        public ActionResult ReportInit(int c_id, int w_id)
        {
            try
            {
                var _context = new DataReportContext(APP);
                var Worker = _context.Worker.Where(w => w.CompanyId == c_id).Select(s => new { Id = s.Id, Value = s.Name }).OrderBy(o => o.Value);
                var Rep_Type = _context.Rep_Type.Select(s => new { Id = s.Id, Value = s.Value }).OrderBy(o => o.Value);

                var drugclass = new DrugClassifierContext(APP);
                var INN= drugclass.INNs.Select(s => new { Id = s.Value, Value = s.Value }).OrderBy(o => o.Value);
                var TN = drugclass.TradeNames.Select(s => new { Id = s.Value, Value = s.Value }).OrderBy(o => o.Value);

                var gz_con = new GovernmentPurchasesContext(APP);
                var Regions=gz_con.RegionName.Select(s => new { Id = s.Name, Value = s.Name }).OrderBy(o => o.Value);
                //
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult_reportInit() { Workers = Worker.ToList(), Rep_Type= Rep_Type.ToList(), INN= INN.ToList(), Regions= Regions.ToList(), TN= TN.ToList(), status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Authorize(Roles = "Clients_Reports")]
        public ActionResult ReportsGet(int c_id,int w_id)
        {
            try
            {
                var _context = new DataReportContext(APP);
                var ret = _context.Rep_Param.Where(w=>w.WorkerId== w_id).Select(s => s).OrderBy(o => o.Name);
                //
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = ret, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Report_Save(ICollection<DataAggregator.Domain.Model.DataReport.Rep_Param> array)
        {
            try
            {
                var _context = new DataReportContext(APP);
                foreach (var item in array)
                {
                    item.IsNull();
                    DataAggregator.Domain.Model.DataReport.Rep_Param upd = new Domain.Model.DataReport.Rep_Param();
                    if (item.Id > 0)
                    {
                        upd = _context.Rep_Param.Where(w => w.Id == item.Id).Single();

                    }
                    else
                    {
                        upd = new Domain.Model.DataReport.Rep_Param();
                        upd.Create = DateTime.Now;
                        upd.LastSend = new DateTime(2019, 1, 1);
                    }
                    upd.IsActive = item.IsActive;
                    upd.Name = item.Name;
                    upd.Param_ATCEphmra = item.Param_ATCEphmra;
                    upd.Param_INN = item.Param_INN;
                    upd.Param_Region_Customer = item.Param_Region_Customer;
                    upd.Param_Region_Receiver = item.Param_Region_Receiver;
                    upd.Param_Customer_INN = item.Param_Customer_INN;
                    upd.Param_TN = item.Param_TN;
                    upd.Param_word = item.Param_word;
                    upd.Period = item.Period;
                    upd.Rep_TypeId = item.Rep_TypeId;
                    upd.WorkerId = item.WorkerId;
                    if (item.Id == 0)
                        _context.Rep_Param.Add(upd);
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
                return BadRequest(e.Message);
            }
        }

    }
    public class JsonResult_reportInit
    {
        public object Workers { get; set; }
        public object Rep_Type { get; set; }
        public object TN { get; set; }
        public object INN { get; set; }
        public object Regions { get; set; }

        public string status { get; set; }
        public bool Success { get; set; }
    }
    public class JsonResult
    {
        public object Data { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public bool Success { get; set; }
    }
    public class FilterCompany
    {
       public int? id { get; set; }
        public string common { get; set; }
    }
}