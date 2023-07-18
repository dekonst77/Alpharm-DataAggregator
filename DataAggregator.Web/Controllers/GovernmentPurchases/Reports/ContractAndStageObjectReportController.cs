using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using DataAggregator.Web.Models.GovernmentPurchases.CalculatedDataEditor;
using System.Data.SqlClient;
using System.Data;

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
                    object result = null;
                    result = context.Database.SqlQuery<DataAggregator.Domain.Model.GovernmentPurchases.ContractAndStageObjectReport>(
                    @"EXEC report.ContractAndStageObject_Report @Number = @Number, @ReestNumber = @ReestNumber"
                        , new SqlParameter { ParameterName = "@Number", SqlDbType = SqlDbType.VarChar, Value = PurchaseNumber }
                        , new SqlParameter { ParameterName = "@ReestNumber", SqlDbType = SqlDbType.VarChar, Value = ReestrNumber }
                        ).ToList();
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