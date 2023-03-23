using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace DataAggregator.Web.Controllers.Classifier
{
    /// <summary>
    /// Модуль для добавления ДОП ассортимента в БД мониторинг
    /// Разработка #11668
    /// </summary>
    [Authorize(Roles = "SPharmacist")]
    public class DOPMonitoringDatabaseController : BaseController
    {
        private readonly DrugClassifierContext _context;

        public DOPMonitoringDatabaseController()
        {
            _context = new DrugClassifierContext(APP);
        }

        ~DOPMonitoringDatabaseController()
        {
            _context.Dispose();
        }

        // GET: DOPMonitoringDatabase
        public ActionResult Init()
        {
            try
            {
                var result = _context.GetDOPBlockingForMonitoringDatabase_Result().ToList();
                ViewData["DOPBlocking"] = result;

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public ActionResult InitBlocking()
        {
            try
            {
                var result = _context.GetBlocking_Result().ToList();
                ViewData["Blocking"] = result;

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult GetGoodsCategoryList()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    return ReturnData(context.GoodsCategory.Include(t => t.GoodsSection)
                        .OrderBy(c => c.GoodsSection.Name).ThenBy(c => c.Name).ToList());
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

    }
}