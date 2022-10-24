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
    public class BudjetController : BaseController
    {
        public ActionResult Budjet_Init()
        {
            try
            {
                var _context_GS = new GSContext(APP);
                var _context = new GovernmentPurchasesContext(APP);


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
    }
}