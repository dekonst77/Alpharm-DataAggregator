using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierCheckReport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier.Reports
{
    public class CheckClassifireReportController : BaseController
    {
        private readonly DrugClassifierContext _context;

        public CheckClassifireReportController()
        {
            _context = new DrugClassifierContext(APP);
        }

        ~CheckClassifireReportController()
        {
            _context.Dispose();
        }
        [HttpPost]
        public JsonResult CheckClassifireReportView()
        {

            var result = _context.ClassifierCheckReport_SP("ExceptionList").ToList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult CheckClassifireReport_To_Excel()
        {
            var cmd = _context.Database.Connection.CreateCommand();
            cmd.CommandText = "[report].[ClassifierCheckReport]";

            try
            {
                _context.Database.Connection.Open();

                var reader = cmd.ExecuteReader();

                // 1 Лист - список исключений
                var exceptionList = ((IObjectContextAdapter)_context).ObjectContext.Translate<ClassifierCheckReportExceptionListResult>(reader).ToList();

                // 2 лист - TN+Brand
                reader.NextResult();
                var TNBrandList = ((IObjectContextAdapter)_context).ObjectContext.Translate<ClassifierCheckReportTNBrandListResult>(reader).ToList();

                // 3 лист - ATCWhoDescription
                reader.NextResult();
                var ATCWhoDescriptionList = ((IObjectContextAdapter)_context).ObjectContext.Translate<ClassifierCheckReportATCWhoDescriptionListResult>(reader).ToList();

                // 4 лист - FTG
                reader.NextResult();
                var FTGList = ((IObjectContextAdapter)_context).ObjectContext.Translate<ClassifierCheckReportFTGListResult>(reader).ToList();

                // 5 лист - ATCEphmraDescription
                reader.NextResult();
                var ATCEphmraDescriptionList = ((IObjectContextAdapter)_context).ObjectContext.Translate<ClassifierCheckReportATCEphmraDescriptionListResult>(reader).ToList();

                // 6 лист - FormProduct
                reader.NextResult();
                var FormProductList = ((IObjectContextAdapter)_context).ObjectContext.Translate<ClassifierCheckReportFormProductListResult>(reader).ToList();

                Excel.Excel excel = new Excel.Excel();
                excel.Create();

                excel.InsertDataTable("Список исключений", 1, 1, exceptionList, true, true, null);
                excel.InsertDataTable("TN+Brand", 1, 1, TNBrandList, true, true, null);
                excel.InsertDataTable("ATCWhoDescription", 1, 1, ATCWhoDescriptionList, true, true, null);
                excel.InsertDataTable("FTG", 1, 1, FTGList, true, true, null);
                excel.InsertDataTable("ATCEphmraDescription", 1, 1, ATCEphmraDescriptionList, true, true, null);
                excel.InsertDataTable("FormProduct", 1, 1, FormProductList, true, true, null);

                byte[] bb = excel.SaveAsByte();

                return File(bb, "application/vnd.ms-excel");
            }
            finally
            {
                _context.Database.Connection.Close();
            }
        }

        [HttpPost]
        public ActionResult RegistrationCertificateNumberLoad()
        {
            var RegistrationCertificateNumberList = _context.RegistrationCertificates.Select(t => new { t.Id, Value = t.Number }).ToList();
            ViewBag.RegistrationCertificateNumberList = RegistrationCertificateNumberList;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = ViewBag }
            };
            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult CheckClassifireReportLoad()
        {
            var CheckClassifireReportList = _context.ClassifierReport.Select(t => new { t.Id, Value = t.ReportCode }).ToList();
            ViewBag.CheckClassifireReportList = CheckClassifireReportList;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = ViewBag }
            };
            return jsonNetResult;
        }

    }
}