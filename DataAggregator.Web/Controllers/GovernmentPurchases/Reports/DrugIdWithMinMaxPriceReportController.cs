using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace DataAggregator.Web.Controllers.GovernmentPurchases.Reports
{
    [Authorize(Roles = "GManager, GOperator")]
    public class DrugIdWithMinMaxPriceReportController : BaseController
    {

        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~DrugIdWithMinMaxPriceReportController()
        {
            _context.Dispose();
        }
        
        /// <summary>
        /// Отчёт
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetReport(DateTime DateStart, DateTime DateEnd)
        {
            var result = new Dictionary<string, object>();
            
            _context.Database.CommandTimeout = 0;
			//var reportData = _context.DrugIdWithMinMaxPriceView.Where(w=>w.).Take(50000).ToList();
			string sql = string.Format(@"select * from (	
	select 
		'Закупка' as Source, 
		class.DrugId as DrugId, 
		class.TradeName as TradeName, 
		class.DrugDescription AS DrugDescription,
		min(poc.Price) as MinPrice, 
		max(poc.Price) as MaxPrice, 
		ROUND(max(poc.Price)/iif(min(poc.Price) = 0, null, min(poc.Price)), 2) as Coeff
	from dbo.Purchase as p with(nolock)
	inner join dbo.Lot as l with(nolock) on p.Id = l.PurchaseId
	inner join dbo.PurchaseObjectReady as por with(nolock) on l.Id = por.LotId
	inner join calc.PurchaseObjectCalculated as poc with(nolock) on por.Id = poc.PurchaseObjectReadyId
	inner join DrugClassifier.Classifier.ExternalView_FULL AS class WITH (nolock) ON class.ClassifierId = por.ClassifierId
	where [VNC]=0
	and p.DateBegin between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'
	group by class.DrugId, class.TradeName, class.DrugDescription
	having max(poc.Price) >= min(poc.Price) * 10

	union

	select 
		'Контракт' as Source,
		class.DrugId as DrugId, 
		class.TradeName as TradeName, 
		class.DrugDescription AS DrugDescription,
		min(coc.Price) as MinPrice, 
		max(coc.Price) as MaxPrice, 
		ROUND(max(coc.Price)/iif(min(coc.Price) = 0, null, min(coc.Price)), 2) as Coeff
	from dbo.Purchase as p with(nolock)
	inner join dbo.Lot as l with(nolock) on p.Id = l.PurchaseId
	inner join dbo.Contract as c with(nolock) on l.Id = c.LotId
	inner join dbo.ContractObjectReady as cor with(nolock) on c.Id = cor.ContractId
	inner join calc.ContractObjectCalculated as coc with(nolock) on cor.Id = coc.ContractObjectReadyId
	inner join DrugClassifier.Classifier.ExternalView_FULL AS class WITH (nolock) ON class.ClassifierId = cor.ClassifierId
	where [VNC]=0
	and p.DateBegin between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}'
	group by class.DrugId, class.TradeName, class.DrugDescription
	having max(coc.Price) >= min(coc.Price) * 10	
	) t", DateStart, DateEnd);

			var reportData = _context.Database.SqlQuery<DataAggregator.Domain.Model.GovernmentPurchases.DrugIdWithMinMaxPriceView>(sql).ToList();

            result.Add("reportData", reportData);
            result.Add("count", reportData.Count);            

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }
    }
}