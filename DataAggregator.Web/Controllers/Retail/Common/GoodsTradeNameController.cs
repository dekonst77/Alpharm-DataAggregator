using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail.Common
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class GoodsTradeNameController : BaseController
    {
        [HttpPost]
        public async Task<JsonResult> SearchGoodsTradeName(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            using (var context = new DrugClassifierContext(APP))
            {
                List<GoodsTradeName> goodsTradeNames = await context.GoodsTradeName.Where(s => values.Any(v => s.Value.Contains(v))).ToListAsync();

                List<Domain.Model.Common.DictionaryItem> result =
                    goodsTradeNames
                        .Select(s => new Domain.Model.Common.DictionaryItem { Id = s.Id, Value = s.Value })
                        .ToList();

                return Json(result);
            }
        }
    }
}