using DataAggregator.Core;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers
{
    public class DictionaryController : BaseController
    {
        [HttpPost]
        public ActionResult GetDictionary(string value, string dictionary, int? count)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                List<DictionaryItem> values = DictionaryData.GetData(context, dictionary, value, count).ToList();
                return new JsonNetResult(values);
            }
        }

        [HttpPost]
        public ActionResult GetDrugTypes()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                List<DictionaryJson> values = context.DrugType.OrderBy(d => d.Id).Select(c => new DictionaryJson() { Id = c.Id, Value = c.Value }).ToList();
                return new JsonNetResult(values);
            }
        }

        [HttpPost]
        public ActionResult LoadProductionStageList()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var values = context.ProductionStage.ToList().Select(ps => new DictionaryJson(ps));
                return new JsonNetResult(values);
            }
        }

        [HttpPost]
        public ActionResult GetLocalizationByManufacturer(long Id)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                DictionaryItem values = context.GetLocalizationByManufacturerTable(Id).Select(t => new DictionaryItem() { Id = t.Id.GetValueOrDefault(), Value = t.Value }).FirstOrDefault();
                return new JsonNetResult(values);
            }
        }
    }
}