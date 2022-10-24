using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail.View;
using DataAggregator.Web.Models.Retail.PriceRuleEditor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DataAggregator.Web.Models.Retail.CommonPriceRuleEditor;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class RegionController : BaseController
    {
        [HttpPost]
        public async Task<JsonResult> SearchRegion(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            List<PriceRuleModelDictionary> result;

            using (var context = new RetailContext(APP))
            {
                List<RegionPM12View> regions = await context.SearchRegionAsync(values);

                result = regions.Select(CreateRegionModel).ToList();
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> SearchRegionPM01(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            List<PriceRuleModelDictionary> result;

            using (var context = new RetailContext())
            {
                List<RegionPM01View> regions = await context.SearchRegionPM01Async(values);

                result = regions.Select(CreateRegionModel).ToList();
            }

            return Json(result);
        }
        

        public static PriceRuleModelDictionary CreateRegionModel(RegionPM12View region)
        {
            return new PriceRuleModelDictionary
            {
                RegionCode = region.RegionPM12,
                Region = GetRegionFullName(region)
            };
        }

        public static PriceRuleModelDictionary CreateRegionModel(RegionPM01View region)
        {
            return new PriceRuleModelDictionary
            {
                RegionCode = region.RegionPM01,
                Region = string.Format("{0} {1}", region.RegionPM01, region.FullName)
        };
        }

        private static string GetRegionFullName(RegionPM12View region)
        {
            return string.Format("{0} {1}", region.RegionPM12, region.FullName);
        }
    }
}