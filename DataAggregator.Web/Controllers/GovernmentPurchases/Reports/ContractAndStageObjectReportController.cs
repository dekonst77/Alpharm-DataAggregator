using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace DataAggregator.Web.Controllers.GovernmentPurchases.Reports
{
    [Authorize(Roles = "GManager, GOperator")]
    public class ContractAndStageObjectReportController : BaseController
    {
        [HttpPost]
        public ActionResult GetReport(string PurchaseNumber, string ReestrNumber)
        {

            try
            {
                using (var context = new GovernmentPurchasesContext(APP))
                {
                    string sql = string.Format(@"EXEC report.ContractAndStageObject_tmp @Number = '{0}', @ReestNumber = '{1}'", PurchaseNumber, ReestrNumber);

                    object result = null;
                    result = context.Database.SqlQuery<DataAggregator.Domain.Model.GovernmentPurchases.ContractAndStageObjectReport>(sql).ToList();
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