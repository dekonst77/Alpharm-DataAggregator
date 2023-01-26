using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.MassFixesData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace DataAggregator.Web.Controllers.Systematization
{
    [Authorize(Roles = "SBoss")]
    public class PrioritetWordsController : BaseController
    {
        public ActionResult Init()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                var list = _context.Source.OrderBy(o => o.Name).ToList();
                ViewData["Source"] = list;

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
        public ActionResult PrioritetWords_search()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);

                var list = _context.PrioritetWordsWithQueueView.OrderBy(o => o.Name).ToList();
                ViewData["PrioritetWords"] = list;

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
        public ActionResult PrioritetWords_save(
            ICollection<DataAggregator.Domain.Model.DrugClassifier.Systematization.PrioritetWords> array_PrioritetWords
            )
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                if (array_PrioritetWords != null)
                    foreach (var item in array_PrioritetWords)
                    {
                        if (item.Id > 0)
                        {
                            var upd = _context.PrioritetWords.Where(w => w.Id == item.Id).Single();
                            upd.Name = item.Name;
                            upd.Value = item.Value;
                        }
                        else
                        {
                            _context.PrioritetWords.Add(new Domain.Model.DrugClassifier.Systematization.PrioritetWords() { Name = item.Name, SourceId = item.SourceId, Value = item.Value });
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
    }
}