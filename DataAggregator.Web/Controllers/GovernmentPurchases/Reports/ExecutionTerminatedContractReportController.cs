using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using DataAggregator.Domain.DAL;

namespace DataAggregator.Web.Controllers.GovernmentPurchases.Reports
{
    [Authorize(Roles = "GManager, GOperator")]
    public class ExecutionTerminatedContractReportController : BaseController
    {
        [HttpPost]
        public ActionResult GetReport(DateTime DateStart, DateTime DateEnd, string ReportObject)
        {

            try
            {
                using (var context = new GovernmentPurchasesContext(APP))
                {
                    //base.LogError(new ApplicationException("1"));
                    DateTime dateStart = DateStart.Date;
                    DateTime dateEnd = DateEnd.Date.AddDays(1);//чтобы обработать всё до конца суток
                    string reportObject = ReportObject;
                    context.Database.CommandTimeout = 0;

                    object result = null;
                    result = context.ExecutionTerminatedContractView.Where(w => w.DateBegin >= dateStart && w.DateBegin < dateEnd).Take(50000).ToList();
                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = result
                    };
                    return jsonNetResult;
                }
            }
            catch (Exception e)
            {
                string message = string.Empty;

                Exception exc = e;
                while (exc != null)
                {
                    message += exc.Message;
                    exc = exc.InnerException;
                }
                base.LogError(e);

                return BadRequest(message);
            }
        }
    }
}