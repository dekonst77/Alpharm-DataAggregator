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
using DataAggregator.Domain.Model.GovernmentPurchases;


namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class GZController : BaseController
    {
        public ActionResult AutoNature_Init()
        {
            try
            {
                var _context_GS = new GSContext(APP);
                var _context = new GovernmentPurchasesContext(APP);
                ViewData["Bricks_L3"] = _context_GS.Bricks_L3.OrderBy(o => o.Value).ToList();
                ViewData["Nature"] = _context.Nature.OrderBy(o => o.Name).Select(s => new { s.Id, s.Name, s.NameMini, CategoryName = s.Category.Name }).ToList();
                ViewData["Nature_L2"] = _context.Nature_L2.OrderBy(o => o.Name).Select(s => new { s.Id, s.Name }).ToList();
                ViewData["Category"] = _context.Category.OrderBy(o => o.Name).ToList();
                ViewData["Funding"] = _context.Funding.OrderBy(o => o.Name).Select(s => new { s.Id, s.Name }).ToList();

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
        public ActionResult AutoNature_search()
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);

                ViewData["AutoNature_Text"] = _context.AutoNature_Text.ToList();
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
        public ActionResult AutoNature_save(
            ICollection<AutoNature_Text> array_UPD
            )
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);
                if (array_UPD != null)
                    foreach (var item in array_UPD)
                    {
                        if (item.NatureId ==0)
                            item.NatureId = null;
                        if (item.Nature_L2Id == 0)
                            item.Nature_L2Id = null;
                        if (item.FundingId == 0)
                            item.FundingId = null;
                        if (item.Id == 0)//Новая
                        {
                            var NN = _context.AutoNature_Text.Add(new AutoNature_Text()
                            {
                                Comment = item.Comment,
                                Customer_Bricks_L3 = item.Customer_Bricks_L3,
                                FundingId = item.FundingId,
                                IsInName = item.IsInName,
                                NatureId = item.NatureId,
                                Nature_L2Id = item.Nature_L2Id,
                                Value = item.Value
                            });
                            item.Id = NN.Id;
                        }
                        else
                        {
                            if (item.Id < 0)//Удаление
                            {
                                var DEL = _context.AutoNature_Text.Where(w => w.Id == -1 * item.Id).FirstOrDefault();
                                _context.AutoNature_Text.Remove(DEL);
                            }
                            else
                            {//Обновление
                                var UPD = _context.AutoNature_Text.Where(w => w.Id == item.Id).FirstOrDefault();
                                if (UPD == null)
                                {
                                }
                                else
                                {
                                    UPD.Value = item.Value;
                                    UPD.IsInName = item.IsInName;
                                    UPD.Customer_Bricks_L3 = item.Customer_Bricks_L3;
                                    UPD.Comment = item.Comment;
                                    UPD.NatureId = item.NatureId;
                                    UPD.Nature_L2Id = item.Nature_L2Id;
                                    UPD.FundingId = item.FundingId;
                                }
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
    }
}