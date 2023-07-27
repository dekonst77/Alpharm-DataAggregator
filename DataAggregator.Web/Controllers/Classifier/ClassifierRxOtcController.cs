using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class ClassifierRxOtcController : BaseController
    {
        private readonly DrugClassifierContext _context;

        public ClassifierRxOtcController()
        {
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult Init(bool Used, bool Excluded)
        {
            try
            {
                var result = _context.LoadClassifierRxOtc_SP_Result(Used, Excluded).Take(100).ToList();
                ViewData["RxOtc"] = result;

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

    }
}