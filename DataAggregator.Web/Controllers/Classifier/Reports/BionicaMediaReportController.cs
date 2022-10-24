using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier.Reports
{
    [Authorize(Roles = "SBoss, SPharmacist")]
    public class BionicaMediaReportController : BaseController
    {
        [HttpPost]
        public ActionResult GetReport(int year, int month, int[] tradeNameIds, int[] ownerTradeMarkIds, int[] packerIds)
        {
            using (var context = new RetailContext(APP))
            {
                var result = new Dictionary<string, object>();

                context.Database.CommandTimeout = 0;

                var reportData =
                    context.Database.SqlQuery<BionicaMediaReport>(GenerateQueryText(true, year - 2000, month,tradeNameIds,ownerTradeMarkIds, packerIds)).ToList();

                result.Add("reportData", reportData);
                result.Add("count", reportData.Count);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };

                return jsonNetResult;
            }
        }

        [HttpPost]
        public FileResult ExportAllToCsv(int year, int month)
        {
            using (var context = new RetailContext())
            {
                const char _delimiter = '|';

                using (var sw = new StringWriter())
                {
                    var properties = typeof(BionicaMediaReport).GetProperties();
                    var header = properties.Select(n => n.Name).Aggregate((a, b) => a + _delimiter + b);
                    sw.WriteLine(header);

                    var reportData =
                        context.Database.SqlQuery<BionicaMediaReport>(GenerateQueryText(false, year - 2000, month, null,
                            null, null)).ToList();

                    string row = "";

                    foreach (var fcwp in reportData)
                    {
                        if (fcwp != null)
                        {
                            row = properties
                                .Select(p => p.GetValue(fcwp))
                                .Select(val => val == null
                                    ? ""
                                    : val is bool
                                        ? (bool)val
                                            ? "1"
                                            : "0"
                                        : val.ToString())
                                .Aggregate((a, b) => a + _delimiter + b);
                        }

                        sw.WriteLine(row);
                    }

                    byte[] buffer = System.Text.Encoding.GetEncoding(1251).GetBytes(sw.ToString());

                    return File(buffer, System.Net.Mime.MediaTypeNames.Application.Octet);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> SearchTradeName(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            using (var context = new DrugClassifierContext(APP))
            {
                List<TradeName> tradeNames = await context.TradeNames.Where(s => values.Any(v => s.Value.Contains(v))).ToListAsync();

                List<Domain.Model.Common.DictionaryItem> result =
                    tradeNames
                        .Select(s => new Domain.Model.Common.DictionaryItem { Id = s.Id, Value = s.Value })
                        .ToList();

                return Json(result);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SearchManufacturer(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            using (var context = new DrugClassifierContext(APP))
            {
                List<Manufacturer> manufacturers = await context.Manufacturer.Where(s => values.Any(v => s.Value.Contains(v))).ToListAsync();

                List<Domain.Model.Common.DictionaryItem> result =
                    manufacturers
                        .Select(s => new Domain.Model.Common.DictionaryItem { Id = s.Id, Value = s.Value })
                        .ToList();

                return Json(result);
            }
        }

        /// <summary>
        /// Формирует текст запроса к БД
        /// </summary>
        /// <param name="forVisualView">Формируем запрос для визуального представления в гриде или для выгрузки в файл csv?</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="tradeNameIds">Массив с Id торговых наименований</param>
        /// <param name="ownerTradeMarkIds">Массив с Id производителей</param>
        /// <param name="packerIds">Массив с Id упаковщиков</param>
        /// <returns></returns>
        private string GenerateQueryText(bool forVisualView, int year, int month, int[] tradeNameIds, int[] ownerTradeMarkIds, int[] packerIds)
        {
            const int _topCount = 40000;
            string query = string.Format(@" select {0}f.*, t.[Year], t.[Month], t.SellingPrice 
                                            from [s-sql1].DrugClassifier.Classifier.FullClassifier as f
                                            {1} join (
	                                            SELECT 
	                                                ev.DrugId as DrugId
		                                            ,ev.OwnerTradeMarkId as OwnerTradeMarkId
		                                            ,ev.PackerId as PackerId
		                                            ,rd.[Year]
		                                            ,rd.[Month]
		                                            ,SUM(rd.SellingSumNDS)/SUM(rd.SellingCount) AS SellingPrice
	                                            FROM RetailData.calc.Region0DataView as rd
		                                            INNER JOIN RetailData.Classifier.ClassifierInfoAllPeriod as ciap 
			                                            ON rd.DrugId = ciap.DrugId AND rd.OwnerTradeMarkId = ciap.OwnerTradeMarkId AND rd.PackerId = ciap.PackerId
		                                            INNER JOIN RetailData.Classifier.ExternalViewAllPeriod as ev 
                                                        ON ciap.ClassifierId = ev.ClassifierId
	                                            WHERE ev.IsBad = 0 AND ev.IsOther = 0
	                                                  and Year = {2} and month = {3}
	                                            GROUP BY ev.DrugId, ev.OwnerTradeMarkId, ev.PackerId, rd.[Year], rd.[Month]
                                                HAVING SUM(rd.SellingSumNDS)/SUM(rd.SellingCount) is not null 
                                            ) as t on f.DrugId = t.DrugId and f.OwnerTradeMarkId = t.OwnerTradeMarkId and f.PackerId = t.PackerId
                                            {4}",
                                        forVisualView ? "top " + _topCount + " " : "",
                                        forVisualView ? "inner" : "left",
                                        year,
                                        month,
                                        GetFilterConditions(forVisualView, tradeNameIds, ownerTradeMarkIds, packerIds));
            return query;
        }

        private string GetFilterConditions(bool forVisualView, int[] tradeNameIds, int[] ownerTradeMarkIds, int[] packerIds)
        {
            var whereCondition = new StringBuilder();

            if (tradeNameIds != null && tradeNameIds.Length > 0)
            {
                whereCondition.AppendFormat("WHERE f.TradeNameId in ({0})", JoinIds(tradeNameIds));
            }

            if (ownerTradeMarkIds != null && ownerTradeMarkIds.Length > 0)
            {
                whereCondition.Append(whereCondition.Length == 0 ? "WHERE " : " AND ");
                whereCondition.AppendFormat("f.OwnerTradeMarkId in ({0})", JoinIds(ownerTradeMarkIds));
            }

            if (packerIds != null && packerIds.Length > 0)
            {
                whereCondition.Append(whereCondition.Length == 0 ? "WHERE " : " AND ");
                whereCondition.AppendFormat("f.PackerId in ({0})", JoinIds(packerIds));
            }

            return whereCondition.ToString();
        }

        private static string JoinIds(int[] array)
        {
            return string.Join(", ", array.Select(s => string.Format("{0}", s)));
        }
    }
}