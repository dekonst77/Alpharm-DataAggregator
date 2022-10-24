using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class SearchRawDataByDrugClearController : BaseController
    {
        /// <summary>
        /// Получить данные
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetData(int startYear, int startMonth, int endYear, int endMonth, int[] drugClearIds, string drugName)
        {
            List<AggregatedRawDataByDrugClear> result;

            using (var context = new RetailContext())
            {
                result = await context.SearchRawDataByDrugClear(startYear, startMonth, endYear, endMonth, drugClearIds, drugName);
            }

            ProcessData(result);

            return new JsonNetResult(result);
        }

        private static void ProcessData(IEnumerable<AggregatedRawDataByDrugClear> items)
        {
            const string pathPrefix = @"\\gk.bionika.ru\MSK\HQ\Alpharm\ДРА\Main\RetailData\Current\";

            foreach (AggregatedRawDataByDrugClear item in items)
            {
                item.Path = pathPrefix + item.Path;
                item.PurchasePriceNds = item.PurchaseCount.HasValue && item.PurchaseCount.Value != 0 ? item.PurchaseSumNds / item.PurchaseCount : 0;
                item.SellingPriceNds = item.SellingCount.HasValue && item.SellingCount.Value != 0 ? item.SellingSumNds / item.SellingCount : 0;
            }
        }
    }
}