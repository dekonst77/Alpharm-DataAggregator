using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class BrandController : BaseController
    {
        [HttpPost]
        public async Task<JsonResult> SearchBrand(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            using (var context = new DrugClassifierContext(APP))
            {
                List<Brand> result = await context.Brand.Where(s => s.UseClassifier && values.Any(v => s.Value.Contains(v))).ToListAsync();

                return Json(result);
            }
        }
    }
}