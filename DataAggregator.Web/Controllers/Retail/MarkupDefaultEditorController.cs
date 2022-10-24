using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class MarkupDefaultEditorController : BaseController
    {

        private RetailContext _context;

        public MarkupDefaultEditorController()
        {
            _context = new RetailContext();
        }

        ~MarkupDefaultEditorController()
        {
            _context.Dispose();
        }
        
        /// <summary>
        /// Список наценок по-умолчанию
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetData()
        {
            var result = new Dictionary<string, object>();
            
            _context.Database.CommandTimeout = 3600;
            var data = _context.MarkupDefaultView.ToList();

            foreach (var m in data)
            {
                m.Markup = m.Markup*100 - 100;
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = data
            };

            return jsonNetResult;
        }

        
        [HttpPost]
        public ActionResult SaveMarkupDefault(long id, decimal markup)
        {
            var markupDefault = _context.MarkupDefault.SingleOrDefault(md => md.Id == id);

            if (markupDefault != null)
            {
                markupDefault.Markup = (markup + 100) / 100 ;
            }

            _context.SaveChanges();
            return null;
        }
    }
}