using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Common;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class ExternalViewAllPeriodController : BaseController
    {
        [HttpPost]
        public async Task<JsonResult> SearchExternalViewAllPeriod(string value)
        {
            string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            if (values.Length == 0)
                return Json(new List<object>());

            using (var context = new DrugClassifierContext(APP))
            {
                List<DictionaryItem> result =
                    await context.ExternalViewAllPeriod
                        .Select(s => new DictionaryItem{ Id = s.ClassifierId, Value = s.TradeName + " " + s.DrugDescription + " " + s.OwnerTradeMark + " " + s.Packer })
                        .Where(s => values.Any(v => s.Value.Contains(v)))
                        .ToListAsync();

                return Json(result);
            }
        }
    }
}