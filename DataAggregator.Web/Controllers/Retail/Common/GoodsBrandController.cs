using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail.Common
{
    //[Authorize(Roles = "RBoss, RManager")]
    //public sealed class GoodsBrandController : BaseController
    //{
    //    [HttpPost]
    //    public async Task<JsonResult> SearchGoodsBrand(string value)
    //    {
    //        string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

    //        if (values.Length == 0)   
    //            return Json(new List<object>());

    //        using (var context = new DrugClassifierContext())
    //        {
    //            List<GoodsBrand> result = await context.GoodsBrand.Where(s => values.Any(v => s.Value.Contains(v))).ToListAsync();

    //            return Json(result);
    //        }
    //    }
    //}

    [Authorize(Roles = "RBoss, RManager")]
    public sealed class GoodsBrandController : BaseController
    {
        [HttpPost]
        public JsonResult SearchGoodsBrand(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            using (var context = new DrugClassifierContext(APP))
            {
                List<Brand> result = context.Brand.Where(s => s.UseGoodsClassifier && values.Any(v => s.Value.Contains(v))).ToList();

                return Json(result);
            }
        }
    }
}