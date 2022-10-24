using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases.Reports
{
    [Authorize(Roles = "GManager, GOperator")]
    public class WrongPricesReportController : BaseController
    {

        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~WrongPricesReportController()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Отчёт
        /// </summary>
        [HttpPost]
        public ActionResult GetReport(Models.GovernmentPurchases.Reports.WrongPricesReport.WrongPricesReportFilterJson filter)
        {
            if (filter != null)
            {
                DateTime dateStart = filter.DateStart.Date;
                DateTime dateEnd = filter.DateEnd.Date.AddDays(1);//чтобы обработать всё до конца суток
                bool includeContracts = filter.IncludeContracts;
                bool includePurchases = filter.IncludePurchases;
                decimal lessCoeff = filter.LessCoeff;
                decimal moreCoeff = filter.MoreCoeff;

                _context.Database.CommandTimeout = 0;
                //var reportData = new List<Domain.Model.GovernmentPurchases.WrongPricesView>();
                string QueryC = string.Format(@"SELECT-- top 50000
coc.Id as ObjectCalculatedId,
	    2 AS ObjectTypeId,
		'Контракт' AS ObjectType,
		p.Id as PurchaseId,
	    cast(0 as bigint) as PurchaseObjectCalculatedId,
		coc.Id as ContractObjectCalculatedId,

		p.Number as PurchaseNumber,		
		p.DateBegin,
		n.Name as NatureName,
		l.Sum AS LotSum,
		coc.ClassifierId,
class.DrugId,
class.InnGroup,
cor.VNC,[kofPriceGZotkl],
		cor.Unit AS ObjectReadyUnit,
		cor.Amount AS ObjectReadyAmount,
		cor.Name as ObjectReadyName,
		iif(isnull(capfd.Price, capadb.Price) = 0, 0, round(coc.Price/isnull(capfd.Price, capadb.Price), 2)) as FDPriceCoefficient,
		coc.Price AS ObjectCalculatedPrice,
		isnull(capfd.Price, capadb.Price) AS FDAveragePrice
	FROM calc.ContractObjectCalculated coc WITH (nolock) 
	INNER JOIN ContractObjectReady cor WITH (nolock) ON coc.ContractObjectReadyId = cor.Id
	INNER JOIN Contract c WITH (nolock) ON cor.ContractId = c.Id
	INNER JOIN Lot l WITH (nolock) ON c.LotId = l.Id
	INNER JOIN Purchase p WITH (nolock) ON l.PurchaseId = p.Id
	LEFT JOIN dbo.Organization AS rorg WITH (nolock) ON c.ReceiverId = rorg.Id 
	LEFT JOIN dbo.Region AS r WITH (nolock) ON r.Id = rorg.RegionId 
	LEFT JOIN Nature n WITH (nolock) ON p.NatureId = n.Id
	LEFT JOIN calc.ContractAveragePriceFdView capfd WITH (nolock) 
		ON coc.ClassifierId= capfd.ClassifierId 
		and year(p.DateBegin) = capfd.Year and month(p.DateBegin) = capfd.Month and capfd.FederalDistrictId = r.FederalDistrictId				
	LEFT JOIN calc.ContractAveragePriceAllDbView capadb WITH (nolock) 
		ON coc.ClassifierId= capadb.ClassifierId 
LEFT JOIN DrugClassifier.Classifier.ExternalView_FULL class WITH (nolock) ON coc.ClassifierId = class.ClassifierId
where (
iif(isnull(capfd.Price, capadb.Price) = 0, 0, round(coc.Price/isnull(capfd.Price, capadb.Price), 2))<1.0/isnull([kofPriceGZotkl],5) 
or 
iif(isnull(capfd.Price, capadb.Price) = 0, 0, round(coc.Price/isnull(capfd.Price, capadb.Price), 2))>isnull([kofPriceGZotkl],5))
AND (p.DateBegin>='{0:yyyyMMdd}' and p.DateBegin<='{1:yyyyMMdd}')", dateStart, dateEnd);
                string QueryP = string.Format(@"	SELECT --top 50000
poc.Id as ObjectCalculatedId,
	    	    1 AS ObjectTypeId,
		'Закупка' AS ObjectType,
		p.Id as PurchaseId,
		poc.Id as PurchaseObjectCalculatedId,
		cast(0 as bigint) as ContractObjectCalculatedId,
		p.Number as PurchaseNumber,		
		p.DateBegin,
		n.Name as NatureName,
		l.Sum AS LotSum,
		poc.ClassifierId,
class.DrugId,
class.InnGroup,
VNC,[kofPriceGZotkl],
		por.Unit AS ObjectReadyUnit,
		por.Amount AS ObjectReadyAmount,
		por.Name as ObjectReadyName,

		iif(isnull(papfd.Price, papadb.Price) = 0, 0, round(poc.Price/isnull(papfd.Price, papadb.Price), 2)) as FDPriceCoefficient,

		poc.Price AS ObjectCalculatedPrice,
		isnull(papfd.Price, papadb.Price) AS FDAveragePrice
	FROM calc.PurchaseObjectCalculated poc WITH (nolock) 
	INNER JOIN PurchaseObjectReady por WITH (nolock) ON poc.PurchaseObjectReadyId = por.Id
	INNER JOIN Lot l WITH (nolock) ON por.LotId = l.Id
	INNER JOIN Purchase p WITH (nolock) ON l.PurchaseId = p.Id
	LEFT JOIN dbo.Organization AS rorg WITH (nolock) ON por.ReceiverId = rorg.Id 
	LEFT JOIN dbo.Region AS r WITH (nolock) ON r.Id = rorg.RegionId 
	LEFT JOIN Nature n WITH (nolock) ON p.NatureId = n.Id
	LEFT JOIN calc.PurchaseAveragePriceFDView papfd WITH (nolock) 
		ON poc.ClassifierId= papfd.ClassifierId 
		and year(p.DateBegin) = papfd.Year and month(p.DateBegin) = papfd.Month and papfd.FederalDistrictId = r.FederalDistrictId
	LEFT JOIN calc.PurchaseAveragePriceAllDbView papadb WITH (nolock) 
		ON poc.ClassifierId= papadb.ClassifierId 
LEFT JOIN DrugClassifier.Classifier.ExternalView_FULL class WITH (nolock) ON poc.ClassifierId = class.ClassifierId
where (
iif(isnull(papfd.Price, papadb.Price) = 0, 0, round(poc.Price/isnull(papfd.Price, papadb.Price), 2))<1.0/isnull([kofPriceGZotkl],5) 
or 
iif(isnull(papfd.Price, papadb.Price) = 0, 0, round(poc.Price/isnull(papfd.Price, papadb.Price), 2))>isnull([kofPriceGZotkl],5))
		AND (p.DateBegin>='{0:yyyyMMdd}' and p.DateBegin<='{1:yyyyMMdd}')", dateStart, dateEnd);

                string Query = "";
				if (includeContracts)
					Query = QueryC;
				if (includePurchases)
				{
					if (Query != "")
						Query += " UNION ALL ";
					Query += QueryP;
				}
					//if (includeContracts && includePurchases)
					//{
					//    reportData = _context.WrongPricesView.Where(w => //((w.FDPriceCoefficient >= moreCoeff) || (w.FDPriceCoefficient <= lessCoeff)) && 
					//                                                  (w.DateBegin >= dateStart) && (w.DateBegin <= dateEnd)).Take(50000).ToList();
					//}

					//if (!includeContracts && includePurchases)
					//{
					//    reportData = _context.WrongPricesView.Where(w => //((w.FDPriceCoefficient >= moreCoeff) || (w.FDPriceCoefficient <= lessCoeff)) && 
					//    (w.DateBegin >= dateStart) && (w.DateBegin <= dateEnd)
					//                                                  && (w.ObjectTypeId == 1)).Take(50000).ToList();
					//}
					//if (includeContracts && !includePurchases)
					//{
					//    reportData = _context.WrongPricesView.Where(w => //((w.FDPriceCoefficient >= moreCoeff) || (w.FDPriceCoefficient <= lessCoeff)) && 
					//    (w.DateBegin >= dateStart) && (w.DateBegin <= dateEnd)
					//                                                  && (w.ObjectTypeId == 2)).Take(50000).ToList();
					//}

				var reportData = _context.Database.SqlQuery<Domain.Model.GovernmentPurchases.WrongPricesView>(Query).ToList();
                var result = new Dictionary<string, object>();
                result.Add("reportData", reportData);
                result.Add("count", reportData.Count);

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