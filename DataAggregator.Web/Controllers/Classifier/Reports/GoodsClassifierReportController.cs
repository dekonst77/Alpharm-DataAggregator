using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier.Reports
{
    /// <summary>
    /// Отчет по классификатору доп. ассортимента
    /// </summary>
    [Authorize(Roles = "SPharmacist")]
    public class GoodsClassifierReportController : BaseController
    {
        private readonly DrugClassifierContext _context;

        public GoodsClassifierReportController()
        {
            _context = new DrugClassifierContext(APP);
        }

        ~GoodsClassifierReportController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult Init(int GoodsCategoryId)
        {
            DataTable Raw = new DataTable();

            var conn = _context.Database.Connection;

            try
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "GoodsClassifier.ReportOnClassifierOfAdditionalAssortment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("GoodsCategoryId", GoodsCategoryId));
                    using (var reader = cmd.ExecuteReader())
                    {
                        Raw.Load(reader);
                    }
                }

                ViewData["GoodsClassifierReport"] = Raw;
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Список доп. полей
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getAddColumnGrid(int GoodsCategoryId)
        {
            try
            {
                var columns = _context.ParameterGroup.Where(t => t.GoodsCategoryId == GoodsCategoryId).Select(t => new { field = t.Name }).ToList();
                ViewData["columns"] = columns;

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = ViewData, count = 0, status = "ок", Success = true }
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

        /// <summary>
        /// Импорт в Excel
        /// </summary>
        /// <param name="GoodsCategoryId">Id категории</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportToExcel(int GoodsCategoryId)
        {
            DataTable Raw = new DataTable();

            using (var conn = _context.Database.Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "GoodsClassifier.ReportOnClassifierOfAdditionalAssortment";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("GoodsCategoryId", GoodsCategoryId));

                    using (var reader = cmd.ExecuteReader())
                        Raw.Load(reader);
                }
            }

            using (Excel.Excel excel = new Excel.Excel())
            {
                excel.Create();

                excel.InsertDataTable("Отчёт по классификатору ДОП ассортимента", 1, 1, Raw, true, true, null);

                byte[] bb = excel.SaveAsByte();

                return File(bb, "application/vnd.ms-excel");
            }           
        }

    } // class
} // namespace