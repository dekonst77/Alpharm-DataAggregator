using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Common;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail.Common
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class GoodsExternalViewController : BaseController
    {
        [HttpPost]
        public async Task<JsonResult> SearchGoodsExternalView(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            using (var context = new GoodsDataContext(APP))
            {
                List<DictionaryItem> result =
                    await context.GoodsExternalView
                        .Select(s => new DictionaryItem { Id = s.ClassifierId, Value = s.GoodsTradeName + " " + s.GoodsDescription + " " + s.OwnerTradeMark + " " + s.Packer + " " + s.Brand })
                        .Where(s => values.Any(v => s.Value.Contains(v)))
                        .ToListAsync();

                return Json(result);
            }
        }
    }
}