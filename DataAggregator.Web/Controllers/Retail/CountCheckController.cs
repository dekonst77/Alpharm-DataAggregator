using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class CountCheckController : BaseController
    {
        private readonly RetailContext _context;

        public CountCheckController()
        {
            _context = new RetailContext();
        }

        ~CountCheckController()
        {
            _context.Dispose();
        }

        [HttpGet]
        public ActionResult GetCountCheckList(int month, int year)
        {
            var countCheckList = _context.CountCheckView.Where(ccv => ccv.Year == year && ccv.Month == month).ToList();

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = countCheckList
            };

            return jsonNetResult;
        }

        public ActionResult EditCountCheckRecord(long id, decimal coefficient)
        {
            var countCheckRecord = _context.CountCheck.Single(cc => cc.Id == id);

            countCheckRecord.Coefficient = coefficient;

            _context.SaveChanges();

            _context.Entry(countCheckRecord).Reload();

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = countCheckRecord
            };

            return jsonNetResult;
        }
    }
}