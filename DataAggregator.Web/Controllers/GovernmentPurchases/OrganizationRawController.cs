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
using System.Data.Entity;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    public class OrganizationRawController : BaseController
    {
        public ActionResult OrganizationRaw_Init()
        {
            try
            {
                var contextAgg = new DataAggregator.Domain.DAL.DataAggregatorContext(APP);
                ViewData["UsersAll"] = contextAgg.UserViewAll.ToList();
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
        public ActionResult OrganizationRaw_search(bool IsNotReady)
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);

                var Raw = _context.OrganizationRaw.Where(w => 1==1);
                if (IsNotReady)
                {
                    Raw = Raw.Where(w => w.IsTrash == false && w.OrganizationId == null);
                }


                ViewData["Raw"] = Raw.ToList();

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
        public ActionResult OrganizationRaw_save(
            ICollection<DataAggregator.Domain.Model.GovernmentPurchases.OrganizationRaw> array_Raw
            )
        {
            try
            {
                var _context = new GovernmentPurchasesContext(APP);
                if (array_Raw != null)
                    foreach (var item in array_Raw)
                    {
                        var upd = _context.OrganizationRaw.Where(w => w.Id == item.Id).Single();

                        if (item.OrganizationId == 0) item.OrganizationId = null;
                        if (item.UserId == 0) item.UserId = null;

                        upd.OrganizationId = item.OrganizationId;
                        upd.UserId = item.UserId;
                        upd.IsTrash = item.IsTrash;
                        upd.DateUpdate = DateTime.Now;
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