using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierCheckReport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
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

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public JsonResult CheckClassifireReportView()
        {

            var result = _context.ClassifierCheckReport_SP("ExceptionList").ToList();
            return Json(result);
        }

        [HttpPost]
        public FileContentResult CheckClassifireReport_To_Excel()
        {
            var cmd = _context.Database.Connection.CreateCommand();
            cmd.CommandText = "[report].[ClassifierCheckReport]";

            _context.Database.Connection.Open();

            try
            {
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

        /// <summary>
        /// сохранение списка исключений
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReportExceptionListSave(List<RegCertificateNumberExceptions> ExceptionList)
        {
            List<RegCertificateNumberExceptions> records = new List<RegCertificateNumberExceptions>();

            foreach (var item in ExceptionList)
            {
                RegCertificateNumberExceptions record = _context.RegCertificateNumberExceptions.Find(item.Id);
                if (record != null)
                {
                    record.ClassifierReportId = item.ClassifierReportId;
                    record.RegistrationCertificateId = item.RegistrationCertificateId;
                }
                else
                {
                    record = _context.RegCertificateNumberExceptions.Add(new RegCertificateNumberExceptions()
                    {
                        ClassifierReportId = item.ClassifierReportId,
                        RegistrationCertificateId = item.RegistrationCertificateId
                    });

                }

                if (_context.SaveChanges() > 0)
                {
                    _context.Entry<RegCertificateNumberExceptions>(record).State = (System.Data.Entity.EntityState)EntityState.Detached;
                    records.Add(_context.RegCertificateNumberExceptions.Find(record.Id));
                }
            }

            ViewData["ReportExceptionRecords"] = records;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = ViewData, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }

        /// <summary>
        /// удаление исключения
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReportExceptionListRemove(int ExceptionId)
        {
            RegCertificateNumberExceptions entity = _context.RegCertificateNumberExceptions.Find(ExceptionId);
            _context.RegCertificateNumberExceptions.Remove(entity);
            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult CheckClassifireReport_To_Email()
        {
            const long SourceId = 5; // Отчет проверки классификатора

            // список получателей
            string receipientList;
            using (var _context = new DataReportContext(APP))
            {
                receipientList = String.Join(",", _context.Reports.Where(t => t.SourceId == SourceId).Select(t => t.Recipients).Distinct().ToArray());
            }

            MailMessage message = new MailMessage()
            {
                Body = "Отчет проверки классификатора во вложении xls файла.",
                IsBodyHtml = true,
                Subject = "Отчет проверки классификатора"
            };
            message.From = new MailAddress("report@alpharm.ru", "Альфарм робот №1");
            message.To.Add(receipientList);

            // Add the file attachment to this email message.
            byte[] content = CheckClassifireReport_To_Excel().FileContents;
            MemoryStream file = new MemoryStream(content);

            Attachment data = new Attachment(file, "Отчет проверки классификатора.xls");

            message.Attachments.Add(data);

            //Send the message.
            SmtpClient client = new SmtpClient()
            {
                UseDefaultCredentials = true,
                Host = "mx01.alpharm.ru",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            client.Send(message);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult CheckClassifireReportToEmailOverDBProfile()
        {
            const long SourceId = 5; // Отчет проверки классификатора

            // список получателей
            string receipientList;
            using (var _context = new DataReportContext(APP))
            {
                receipientList = String.Join(";", _context.Reports.Where(t => t.SourceId == SourceId).Select(t => t.Recipients).Distinct().ToArray());
            }

            byte[] content = CheckClassifireReport_To_Excel().FileContents;

            using (SqlConnection Sqlcon = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                Sqlcon.Open();

                SqlCommand cmd = new SqlCommand
                {
                    Connection = Sqlcon,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "[report].[SendClassifierCheckReport]"
                };

                cmd.Parameters.Add(new SqlParameter("@receipientList", SqlDbType.VarChar));
                cmd.Parameters["@receipientList"].Value = receipientList;

                cmd.Parameters.Add(new SqlParameter("@fileContent", SqlDbType.VarBinary));
                cmd.Parameters["@fileContent"].Value = content;

                cmd.ExecuteNonQuery();
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };

            return jsonNetResult;
        }

    }
}