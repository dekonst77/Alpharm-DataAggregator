using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases.Reports
{
    [Authorize(Roles = "GManager, GOperator")]
    public class NotExportedToExternalPurchasesReportController : BaseController
    {
        [HttpPost]
        public ActionResult GetReport(DateTime DateStart, DateTime DateEnd, string ReportObject)
            //Models.GovernmentPurchases.Reports.NotExportedToExternalPurchasesReport.NotExportedToExternalPurchasesReportFilterJson filter)
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
                    if (reportObject.Equals("Purchases"))
                    {
                        result = context.GetNotExportedToExternalLots(dateStart, dateEnd).Take(50000).ToList();
                    }
                    else
                    {
                        result = context.NotExportedToExternalContractsView.Where(w => w.PurchaseDateBegin >= dateStart && w.PurchaseDateBegin < dateEnd).Take(50000).ToList();
                    }
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