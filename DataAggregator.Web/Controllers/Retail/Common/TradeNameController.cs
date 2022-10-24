using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail.Common
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class TradeNameController : BaseController
    {
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
    }
}