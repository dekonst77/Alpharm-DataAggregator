using DataAggregator.Domain.DAL;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.OFD
{
    public class EtalonPriceController : BaseController
    {
        private readonly OFDContext _context;

        public EtalonPriceController()
        {
            _context = new OFDContext(APP);
        }

        ~EtalonPriceController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetList(int year, int month, decimal? devPercent = null, string searchText = null)
        {
            try
            {
                var result = _context.ViewData_SP(year, month, devPercent, searchText).ToList();
                return new JsonNetResult { Data = result };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ReloadAllData(int year, int month)
        {
            try
            {
                _context.Database.CommandTimeout = 5 * 60;
                _context.ReloadAllData_SP(year, month);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Data = new JsonResult() { Data = Data }
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