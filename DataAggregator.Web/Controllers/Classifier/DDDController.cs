using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.Classifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class DDDController : BaseController
    {
        public ActionResult DDD_Norma_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                ViewData["ATCWho"] = _context.ATCWho.Select(s => new { s.Id, s.Value }).OrderBy(o=>o.Value).ToList();
                ViewData["RouteAdministration"] = _context.RouteAdministration.Select(s => new { s.Id, s.Value }).OrderBy(o => o.Value).ToList();
                ViewData["DDD_Units"] = _context.DDD_Units.Select(s => new { s.Id, s.Value }).OrderBy(o => o.Value).ToList();

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
        public ActionResult DDD_Norma_search()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                ViewData["DDD_Norma"] = _context.DDD_Norma.ToList();
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
        public ActionResult DDD_Norma_save(
            ICollection<DDD_Norma> array_SPR
            )
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_SPR != null)
                    foreach (var item in array_SPR)
                    {
                        if (item.Description == null)
                            item.Description = "";
                        var UPD = _context.DDD_Norma.Where(w => w.ATCWhoId == item.ATCWhoId && w.RouteAdministrationId == item.RouteAdministrationId).FirstOrDefault();
                        if (UPD == null)
                        {
                            _context.DDD_Norma.Add(new DDD_Norma() { ATCWhoId = item.ATCWhoId, DDD = item.DDD, RouteAdministrationId = item.RouteAdministrationId, Units = item.Units, Description = item.Description });
                        }
                        else
                        {
                            if (item.DDD == 0)
                            {
                                _context.DDD_Norma.Remove(UPD);
                            }
                            else
                            {
                                UPD.ATCWhoId = item.ATCWhoId;
                                UPD.DDD = item.DDD;
                                UPD.RouteAdministrationId = item.RouteAdministrationId;
                                UPD.Units = item.Units;
                                UPD.Description = item.Description;
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



        public ActionResult DDD_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                _context.Database.ExecuteSqlCommand("exec [Classifier].[DDD_Set]");
                //ViewData["ATCWho"] = _context.ATCWho.Select(s => new { s.Id, s.Value }).OrderBy(o => o.Value).ToList();
                //ViewData["RouteAdministration"] = _context.RouteAdministration.Select(s => new { s.Id, s.Value }).OrderBy(o => o.Value).ToList();
                ViewData["DDD_Units"] = _context.DDD_Units.Select(s => new { s.Id, s.Value }).OrderBy(o => o.Value).ToList();
                ViewData["DDD_Units_Standart"] = _context.DDD_Units_Standart.ToList();
                ViewData["DDD_Formulas"] = _context.DrugClassification.Where(w=>w.DDD_Formula.Contains("!")).Select(s => new { s.DDD_Formula }).OrderBy(o => o.DDD_Formula).Distinct().ToList();

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
        public ActionResult DDD_search()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                ViewData["DDD"] = _context.DDDView.ToList();
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
        public ActionResult DDD_save(
            ICollection<DDDView> array_SPR
            )
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_SPR != null)
                    foreach (var item in array_SPR)
                    {
                        item.IsNull();
                        var UPD = _context.DrugClassification.Where(w => w.DrugId == item.DrugId && w.OwnerTradeMarkId == item.OwnerTradeMarkId).Single();
                        UPD.DDD_chek = item.DDD_chek;
                        UPD.DDD_Comment = item.DDD_Comment;
                        UPD.DDD_Norma = item.DDD_Norma;
                        UPD.DDD_Units = item.DDD_Units;
                        UPD.DDD_Formula = item.DDD_Formula;
                        UPD.DDDs = item.DDDs;
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

        public ActionResult StandardUnits_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                ViewData["EI"] = _context.EI.ToList();

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
        public ActionResult StandardUnits_search()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                _context.Database.ExecuteSqlCommand("exec [Classifier].[DDD_StandardUnits]");
                ViewData["StandardUnits"] = _context.StandardUnitsView.ToList();
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
        public ActionResult StandardUnits_save(
            ICollection<StandardUnitsView> array_SPR
            )
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_SPR != null)
                    foreach (var item in array_SPR)
                    {
                        item.IsNull();
                        var UPD = _context.Drugs.Where(w => w.Id == item.DrugId).Single();
                        UPD.EIId = item.EIId;
                        UPD.StandardUnits_Hand = item.StandardUnits_Hand;
                        UPD.StandardUnits_Ckeck = item.StandardUnits_Ckeck;
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
    }
}