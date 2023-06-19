using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail.SalesSKUbySF;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DataAggregator.Web.Controllers.Retail
{
    public class CustomJsonResult : JsonResult
    {
        private const string _dateFormat = "yyyy-MM-dd";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter
                {
                    DateTimeFormat = _dateFormat
                };
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }

    [Authorize(Roles = "RBoss, RManager, Ecom")]
    public class SalesSKUBySFController : BaseController
    {
        private readonly RetailContext _context;

        public SalesSKUBySFController()
        {
            _context = new RetailContext(APP);
        }

        ~SalesSKUBySFController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public JsonResult ViewSales(int year, short month, short? districtId, short? region_code)
        {
            try
            {
                var result = _context.ViewSalesSKUByFederationSubject_SP(year, month, districtId, region_code).ToList();
                return new CustomJsonResult { Data = result, MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ViewSalesByGroupModel(int year, short month, string region_model, string searchText = null)
        {
            try
            {
                List<RegionGroupModel> regions = JsonSerializer.Deserialize<List<RegionGroupModel>>(region_model);

                var result = _context.ViewSalesSKUByFedSubGroupModel_SP(year, month, regions, false, searchText).ToList();
                return new CustomJsonResult { Data = result, MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult RecalcDistrData(int year, short month)
        {
            try
            {
                _context.RecalcDistrData_SP(year, month);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult RecalcInitialData(int year, short month)
        {
            try
            {
                _context.RecalcInitialData_SP(year, month);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult RecalcOFDData(int year, short month)
        {
            try
            {
                _context.RecalcOFDData_SP(year, month);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult RecalcCalculatedData(int year, short month)
        {
            try
            {
                var currPeriod = new DateTime(year, month, 15);

                _context.SalesCalculationAlgorithmByRegion(currPeriod, String.Empty, false);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult Edit(ViewSalesSKUByFederationSubject_SP_Result record, string fieldname)
        {
            try
            {
                var result = _context.Load_SalesSKUByFederationSubject_Record(record, fieldname);
                ViewData["SalesSKUBySFRecord"] = result.ToArray();
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
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
        public JsonResult Regions_Init()
        {
            try
            {
                var result = _context.Region.Where(u => u.Level == 1).Select(k => new { k.Code, k.Name, k.FederalDistrictId, orderby = -k.FederalDistrictId });
                return new CustomJsonResult { Data = result };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult FederalDistricts_Init()
        {
            try
            {
                var result = _context.FederalDistrict.Select(k => new { k.Id, k.Name }).OrderBy(k => k.Id);
                return new CustomJsonResult { Data = result };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SalesSKUbySF_To_Excel(int year, short month, string region_model, string searchText = null)
        {
            try
            {
                List<RegionGroupModel> regions = JsonSerializer.Deserialize<List<RegionGroupModel>>(region_model);

                DataTable result = _context.Get_SalesSKUbyFedSubGroupModel_ListTable(year, month, regions, searchText);

                Excel.Excel excel = new Excel.Excel();
                excel.Create();

                excel.InsertDataTable("Продажи SKU по СФ", 1, 1, result, true, true, null);

                excel.Style_ColumnBackColor("Продажи SKU по СФ", 17, System.Drawing.Color.Green); // Коэф. Кор.
                excel.Style_ColumnBackColor("Продажи SKU по СФ", 18, System.Drawing.Color.Green); // уп. тек. Старт
                excel.Style_ColumnBackColor("Продажи SKU по СФ", 47, System.Drawing.Color.Green); // Комментарий

                byte[] bb = excel.SaveAsByte();

                string currentperiod = new DateTime(year, month, 1).ToString("yyyy_MM_dd");

                return File(bb, "application/vnd.ms-excel", "SalesSKUbySF_" + currentperiod + ".xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Импорт коэффициентов коррекции
        /// </summary>
        /// <param name="uploads"></param>
        /// <param name="currentperiod"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SalesSKUbySF_from_Excel(IEnumerable<HttpPostedFileBase> uploads, string currentperiod)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    throw new ApplicationException("uploads not set");

                var file = uploads.First();
                string filename = @"\\s-sql3\FileUpload\SalesSKUbySF_from_Excel_" + User.Identity.GetUserId() + ".xlsx";

                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                file.SaveAs(filename);

                _context.SalesSKUbySF_from_Excel(filename, currentperiod);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null }
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Импорт цены по субъектам федерации
        /// </summary>
        /// <param name="uploads"></param>
        /// <param name="currentperiod"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Price_SalesSKUbySF_from_Excel(IEnumerable<HttpPostedFileBase> uploads, string currentperiod)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    throw new ApplicationException("uploads not set");

                var file = uploads.First();
                string filename = @"\\s-sql3\FileUpload\Price_SalesSKUbySF_from_Excel_" + User.Identity.GetUserId() + ".xlsx";

                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                file.SaveAs(filename);

                _context.Price_SalesSKUbySF_from_Excel(filename, currentperiod);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = null }
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Ratings_To_Excel(int year, short month)
        {
            try
            {
                //Trace.WriteLine("Начало выполнения");
                //Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();

                DataTable result1 = _context.GetRatingByRFandBrand_Table(year, month);
                DataTable result2 = _context.GetRatingByRFandOwnerTradeMark_Table(year, month);
                DataTable result3 = _context.GetRatingBySubjectFederationAndBrand(year, month);
                DataTable result4 = _context.GetRatingBySubjectFederationAndOwnerTradeMark(year, month);

                //stopWatch.Stop();
                //TimeSpan ts = stopWatch.Elapsed;
                //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                //Trace.WriteLine("Время выполнения: " + elapsedTime);

                Excel.Excel excel = new Excel.Excel();
                excel.Create();

                excel.InsertDataTable("РФ бренд", 1, 1, result1, true, true, null);
                excel.InsertDataTable("РФ корп", 1, 1, result2, true, true, null);
                excel.InsertDataTable("СФ бренд", 1, 1, result3, true, true, null);
                excel.InsertDataTable("СФ корп", 1, 1, result4, true, true, null);

                //excel.Style_ColumnBackColor("Продажи SKU по СФ", 12, System.Drawing.Color.Green); // Коэф. Кор.
                //excel.Style_ColumnBackColor("Продажи SKU по СФ", 13, System.Drawing.Color.Green); // уп. тек. Старт
                //excel.Style_ColumnBackColor("Продажи SKU по СФ", 47, System.Drawing.Color.Green); // Комментарий

                byte[] bb = excel.SaveAsByte();

                string currentperiod = new DateTime(year, month, 1).ToString("yyyy_MM_dd");

                return File(bb, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Ratings_To_ExcelTask(int year, short month)
        {
            try
            {
                Task<DataTable>[] tasks = new Task<DataTable>[4];

                tasks[0] = new Task<DataTable>(() => _context.GetRatingByRFandBrand_Table(year, month));
                tasks[1] = new Task<DataTable>(() => _context.GetRatingByRFandOwnerTradeMark_Table(year, month));
                tasks[2] = new Task<DataTable>(() => _context.GetRatingBySubjectFederationAndBrand(year, month));
                tasks[3] = new Task<DataTable>(() => _context.GetRatingBySubjectFederationAndOwnerTradeMark(year, month));

                foreach (var item in tasks)
                    item.Start();

                Task.WaitAll(tasks);

                Excel.Excel excel = new Excel.Excel();
                excel.Create();

                excel.InsertDataTable("РФ бренд", 1, 1, tasks[0].Result, true, true, null);
                excel.InsertDataTable("РФ корп", 1, 1, tasks[1].Result, true, true, null);
                excel.InsertDataTable("СФ бренд", 1, 1, tasks[2].Result, true, true, null);
                excel.InsertDataTable("СФ корп", 1, 1, tasks[3].Result, true, true, null);

                //excel.Style_ColumnBackColor("Продажи SKU по СФ", 12, System.Drawing.Color.Green); // Коэф. Кор.
                //excel.Style_ColumnBackColor("Продажи SKU по СФ", 13, System.Drawing.Color.Green); // уп. тек. Старт
                //excel.Style_ColumnBackColor("Продажи SKU по СФ", 47, System.Drawing.Color.Green); // Комментарий

                byte[] bb = excel.SaveAsByte();

                string currentperiod = new DateTime(year, month, 1).ToString("yyyy_MM_dd");

                return File(bb, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult PricesByFederalSubjectsToExcel(int year, short month)
        {
            try
            {
                DataTable result = _context.Get_PricesByFederalSubjects_ListTable(year, month);

                Excel.Excel excel = new Excel.Excel();
                excel.Create();

                excel.InsertDataTable("Отчёт", 1, 1, result, true, true, null);

                excel.Style_ColumnBackColor("Отчёт", 1, Color.FromArgb(221, 235, 247)); // ClassifierId
                excel.Style_ColumnBackColor("Отчёт", 2, Color.FromArgb(221, 235, 247)); // Наименование ТН
                excel.Style_ColumnBackColor("Отчёт", 3, Color.FromArgb(221, 235, 247)); // Описание ТН
                excel.Style_ColumnBackColor("Отчёт", 4, Color.FromArgb(221, 235, 247)); // Правообладатель

                for (int i = 5; i <= excel.ColumnCount("Отчёт"); i++)
                {
                    excel.Style_ColumnBackColor("Отчёт", i, Color.FromArgb(226, 239, 218));
                }                

                byte[] bb = excel.SaveAsByte();

                string currentperiod = new DateTime(year, month, 1).ToString("yyyy_MM_dd");

                return File(bb, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}