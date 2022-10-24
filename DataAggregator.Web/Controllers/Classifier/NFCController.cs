using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Web.Models.Classifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{

    [Authorize(Roles = "SBoss")]
    public class NFCController : BaseController
    {
        public ActionResult NFC_Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                ViewData["RouteAdministration"] = _context.RouteAdministration.Select(s => new { Id = s.Id, Value = s.Value + " " + s.Description }).OrderBy(o => o.Value).ToList();
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
        public ActionResult NFC_search()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                
                ViewData["NFC"] = _context.NFCLineView.Where(w=>w.Nfc3Id!=null).ToList();
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
        public ActionResult NFC_save(
            ICollection<NFCLineView> array_NFC
            )
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_NFC != null)
                    foreach (var item in array_NFC)
                    {
                        var UPD = _context.NFC.Where(w => w.Id == item.Nfc3Id).Single();
                        UPD.Description = item.Nfc3Description;
                        UPD.RouteAdministrationId = item.RouteAdministrationId;
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