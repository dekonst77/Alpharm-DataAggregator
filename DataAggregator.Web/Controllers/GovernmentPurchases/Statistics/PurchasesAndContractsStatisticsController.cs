using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases.Reports
{
    [Authorize(Roles = "GManager, GOperator")]
    public class PurchasesAndContractsStatisticsController : BaseController
    {

        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~PurchasesAndContractsStatisticsController()
        {
            _context.Dispose();
        }
        
        /// <summary>
        /// Статистика по закупкам
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStatistics(Models.GovernmentPurchases.Statistics.PurchasesAndContractsStatistics.PurchasesAndContractsStatisticsFilterJson filter)
        {
            if (filter != null)
            {
                DateTime dateStart = filter.DateStart.Date;
                DateTime dateEnd = filter.DateEnd.Date.AddDays(1);//чтобы обработать всё до конца суток
                string statisticsObject = filter.StatisticsObject;

                _context.Database.CommandTimeout = 0;

                var statisticsData = statisticsObject.Equals("Purchases") ? _context.GetPurchasesStatistics(dateStart, dateEnd).Take(50000).ToList() : _context.GetContractsStatistics(dateStart, dateEnd).Take(50000).ToList();

                var result = new Dictionary<string, object>();
                result.Add("reportData", statisticsData);
                result.Add("count", statisticsData.Count);

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };

                return jsonNetResult;
            }

            return null;
        }
    }
}