using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class OrphanController : BaseController
    {
        private DrugClassifierContext _context;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult Load(bool ShowOnlyOrphan)
        {
            _context.ClassifierUpdateOrphan();

            IEnumerable<OrphanView> OrphanList;

            if (ShowOnlyOrphan)
                OrphanList = _context.OrphanView.Where(t => t.IsOrphan == ShowOnlyOrphan).OrderBy(t => t.Id).ToList();
            else
                OrphanList = _context.OrphanView.OrderBy(t => t.Id).ToList();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = OrphanList
            };
        }

        [HttpPost]
        public ActionResult Save(ICollection<OrphanView> array_UPD)
        {
            if (array_UPD == null)
                return BadRequest("Нет изменеий");

            List<OrphanView> records = new List<OrphanView>();

            try
            {
                var _context = new DrugClassifierContext(APP);

                foreach (var item in array_UPD)
                {
                    var record = _context.OrphanView.Find(item.Id);
                    record.InDecreeRussianGovernment = item.InDecreeRussianGovernment;
                    record.InListHealthMinistry = item.InListHealthMinistry;
                    record.InGRLS = item.InGRLS;
                    record.InWithoutReg = item.InWithoutReg;

                    records.Add(record);
                }

                if (_context.SaveChanges() > 0)
                {
                    records.ForEach(item =>
                    {
                        _context.Entry<OrphanView>(item).State = EntityState.Detached;
                        item.IsOrphan = _context.OrphanView.Find(item.Id).IsOrphan;
                    });
                }

                ViewData["OrphanRecords"] = records;

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = ViewData, count = array_UPD.Count, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }
}