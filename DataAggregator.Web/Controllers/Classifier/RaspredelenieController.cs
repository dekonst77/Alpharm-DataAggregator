using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.MassFixesData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SPharmacist")]
    public class RaspredelenieController : BaseController
    {
        public ActionResult Raspredelenie_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                ViewData["Raspredelenies"] = _context.Rasp_Raspredelenie.Where(w=>w.IsEnd==false).OrderBy(o=>o.Name).ToList();
                ViewData["Tables"] = _context.Rasp_Tables.OrderBy(o => o.Name).ToList();

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
        public ActionResult Raspredelenie_search(int RaspredelenieId)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                var result = _context.Rasp_DataView.Where(w => w.RaspredelenieId== RaspredelenieId);
                ViewData["result"] = result.ToList();
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
        public ActionResult Raspredelenie_Save(ICollection<DataAggregator.Domain.Model.DrugClassifier.Rasp.DataView> array)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array != null)
                {
                    int RaspredelenieId = array.First().RaspredelenieId;
                    var rasp = _context.Rasp_Raspredelenie.Where(w => w.Id == RaspredelenieId).Single();
                    if (rasp.IsEnd)
                    {
                        return BadRequest("Это распределение уже закрыто! Редактирование запрещено!");
                    }
                    else
                    {

                        foreach (var item in array)
                        {
                            var UPD = _context.Rasp_Data.Where(w => w.Id == item.Id).Single();
                            UPD.ClassifierId_After = item.ClassifierId_After;
                            UPD.UserId = item.UserId;
                        }
                    }
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
        public ActionResult Raspredelenie_Create(string Name, int TableId, DateTime Date_Begin, DateTime Date_End, bool withRegion)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                _context.CreateNew(Name, TableId, Date_Begin, Date_End, withRegion,0);

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
        public ActionResult Raspredelenie_Update(int Raspredelenie_Id)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                _context.CreateNew("", 0, DateTime.Now, DateTime.Now, false, Raspredelenie_Id);

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
        public ActionResult Raspredelenie_Close(int Raspredelenie_Id)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                var RSP = _context.Rasp_Raspredelenie.Where(w => w.Id == Raspredelenie_Id).Single();
                RSP.IsEnd = true;
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
    }
}