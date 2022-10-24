using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.MassFixesData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class KBKController : BaseController
    {
        public ActionResult KBK_Init()
        {
            try
            {
                var _context_GS = new GSContext(APP);
                var _context = new GovernmentPurchasesContext(APP);
                
                ViewData["Bricks_L3"] = _context_GS.Bricks_L3.Where(w=>w.L3_id>0).OrderBy(o => o.Value).ToList();
                ViewData["Nature"] = _context.Nature.OrderBy(o => o.Name).Select(s=>new { s.Id,s.Name,s.NameMini,CategoryName=s.Category.Name}).ToList();
                ViewData["Nature_L2"] = _context.Nature_L2.OrderBy(o => o.Name).Select(s => new { s.Id, s.Name }).ToList();
                ViewData["Category"] = _context.Category.OrderBy(o => o.Name).ToList();
                ViewData["Funding"] = _context.Funding.OrderBy(o => o.Name).ToList();

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
        public ActionResult KBK_search()
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);
                
                var KBK = _context.KBK.Where(w => w.IsUse == true).OrderBy(o => o.Id);
                ViewData["KBK"] = KBK.ToList();
                //ViewData["KBK_Funding"] = _context.KBK_Funding.ToList();
                ViewData["KBK_Main_Rasp"] = KBK.Select(s => s.KBK_Main_Rasp).Distinct().ToList();
                ViewData["KBK_ZS"] = KBK.Select(s => s.KBK_ZS).Distinct().ToList();
                ViewData["KBK_Razdel"] = KBK.Select(s => s.KBK_Razdel).Distinct().ToList();
                ViewData["KBK_Razdel2"] = KBK.Select(s => s.KBK_Razdel2).Distinct().ToList();
                ViewData["KBK_KodVidRashod"] = KBK.Select(s => s.KBK_KodVidRashod).Distinct().ToList();
                var Data = new JsonResultData() {Data=ViewData, status = "ок", Success = true };
               
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
        public ActionResult KBK_save(
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.KBK> array_KBK,
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.KBK_Main_Rasp> array_KBK_Main_Rasp,
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.KBK_ZS> array_KBK_ZS,
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.KBK_Razdel> array_KBK_Razdel,
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.KBK_Razdel2> array_KBK_Razdel2,
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.KBK_KodVidRashod> array_KBK_KodVidRashod
            )
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);
                if (array_KBK != null)
                    foreach (var item in array_KBK)
                    {
                        var upd = _context.KBK.Where(w => w.Id == item.Id && w.Customer_Bricks_L3==item.Customer_Bricks_L3).Single();

                        if (item.NatureId == 0) item.NatureId = null;
                        if (item.Nature_L2Id == 0) item.Nature_L2Id = null;

                        upd.NatureId = item.NatureId;
                        upd.Nature_L2Id = item.Nature_L2Id;
                        upd.Comment = item.Comment;

                        foreach (var delFunding in _context.KBK_Funding.Where(w => w.KBKId == upd.Id && w.Customer_Bricks_L3 == upd.Customer_Bricks_L3))
                        {
                            _context.KBK_Funding.Remove(delFunding);
                        }
                        if (item.KBK_Funding != null)
                        {
                            foreach (var addFunding in item.KBK_Funding)
                            {
                                _context.KBK_Funding.Add(new Domain.Model.GovernmentPurchases.KBK_Funding() { Customer_Bricks_L3 = addFunding.Customer_Bricks_L3, KBKId = addFunding.KBKId, FundingId = addFunding.FundingId });
                            }
                        }
                    }
                if (array_KBK_Main_Rasp != null)
                    foreach (var item in array_KBK_Main_Rasp)
                    {
                        var upd = _context.KBK_Main_Rasp.Where(w => w.Id == item.Id && w.Customer_Bricks_L3 == item.Customer_Bricks_L3).Single();
                        upd.Value = item.Value;
                        upd.Bricks_L3 = item.Bricks_L3;
                    }
                if (array_KBK_ZS != null)
                    foreach (var item in array_KBK_ZS)
                    {
                        var upd = _context.KBK_ZS.Where(w => w.Main_Rasp == item.Main_Rasp && w.ZS == item.ZS && w.Customer_Bricks_L3 == item.Customer_Bricks_L3).Single();
                        if (item.ZS_M1_Value == null) item.ZS_M1_Value = "";
                        if (item.ZS_M2_Value == null) item.ZS_M2_Value = "";
                        if (item.ZS_MM_Value == null) item.ZS_MM_Value = "";
                        if (item.ZS_Napr_Value == null) item.ZS_Napr_Value = "";
                        upd.ZS_M1_Value = item.ZS_M1_Value;
                        upd.ZS_M2_Value = item.ZS_M2_Value;
                        upd.ZS_MM_Value = item.ZS_MM_Value;
                        upd.ZS_Napr_Value = item.ZS_Napr_Value;

                    }
                if (array_KBK_Razdel != null)
                    foreach (var item in array_KBK_Razdel)
                    {
                        var upd = _context.KBK_Razdel.Where(w => w.Id == item.Id).Single();
                        upd.Value = item.Value;
                    }
                if (array_KBK_Razdel2 != null)
                    foreach (var item in array_KBK_Razdel2)
                    {
                        var upd = _context.KBK_Razdel2.Where(w => w.Id == item.Id).Single();
                        upd.Value = item.Value;
                    }
                if (array_KBK_KodVidRashod != null)
                    foreach (var item in array_KBK_KodVidRashod)
                    {
                        var upd = _context.KBK_KodVidRashod.Where(w => w.Id == item.Id).Single();
                        upd.Value = item.Value;
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