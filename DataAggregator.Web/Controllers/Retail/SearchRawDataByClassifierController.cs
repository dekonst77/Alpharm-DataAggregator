using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using DataAggregator.Web.Models.Retail.SearchRawDataByClassifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class SearchRawDataByClassifierController : BaseController
    {
        /// <summary>
        /// Получить данные
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GetData(SearchRawDataByClassifierModel searchRawDataByClassifierModel)
        {
            string query = @"
                with
	                rawData as
	                (
		                select 
                            rd.Year, rd.Month,
			                rd.FileInfoId, ev.ClassifierId, sp.Id as SourcePharmacyId, sp.TargetPharmacyId, r.Code as RegionCode, r.FullName as RegionShortName,
			                sum(rd.PurchaseSumNDS) as PurchaseSumNDS,
			                sum(rd.SellingSumNDS) as SellingSumNDS,
			                sum(rd.PurchaseCount) as PurchaseCount,
			                sum(rd.SellingCount) as SellingCount,
							max(case when spr.SourcePharmacyId is null then 0 else 1 end) as IsSprBlackList
		                from dbo.RawData as rd
		                inner join dbo.SourcePharmacy as sp
			                on sp.Id = rd.SourcePharmacyId
                        inner join dbo.SourceText as st ON rd.SourceTextId = st.Id
		                inner join RetailCalculation.Systematization.DrugClassifier as c
			                on c.Id = st.DrugClassifierId		              
		                inner join Classifier.ExternalViewAllPeriod as ev
			                on c.ClassifierId = ev.ClassifierId
		                inner join dbo.TargetPharmacy as tp
			                on sp.TargetPharmacyId = tp.Id
		                inner join dbo.Region as r
			                on r.Code = tp.RegionPM12
                        left join dbo.SourcePharmacyRelevance spr
							on rd.SourcePharmacyId = spr.SourcePharmacyId and rd.Year = spr.Year and rd.Month = spr.Month
		                where
			                {0}
		                group by
			                rd.Year, rd.Month, rd.FileInfoId, ev.ClassifierId, sp.Id, sp.TargetPharmacyId, r.Code, r.FullName
	                )
                select
	                s.Name as SourceName,
	                rd.FileInfoId,
	                fi.Path,
	                rd.TargetPharmacyId,
			        sum(rd.PurchaseSumNDS) as PurchaseSumNDS,
			        sum(rd.SellingSumNDS) as SellingSumNDS,
			        sum(rd.PurchaseCount) as PurchaseCount,
			        sum(rd.SellingCount) as SellingCount,
                    max(case when tpbl.TargetPharmacyId is null then 0 else 1 end) as IsTpBlackList,
                    max(case when tpbbl.TargetPharmacyId is null then 0 else 1 end) as IsTpBrandBlackList,
                    max(rd.IsSprBlackList) IsSprBlackList,
                    rd.RegionCode + ' ' + rd.RegionShortName as RegionName,
                    {1}
                from
	                rawData rd
                inner join dbo.FileInfo as fi
	                on fi.Id = rd.FileInfoId
                inner join dbo.Source as s
	                on s.Id = fi.SourceId
                inner join Classifier.ExternalViewAllPeriod as ev
                    on rd.ClassifierId = ev.ClassifierId
                left join dbo.TargetPharmacyBlackList tpbl
                    on
                        rd.TargetPharmacyId = tpbl.TargetPharmacyId and rd.Year = tpbl.Year and rd.Month = tpbl.Month
                left join dbo.TargetPharmacyBrandBlackList tpbbl
                    on
                        rd.TargetPharmacyId = tpbbl.TargetPharmacyId and
                        ev.BrandId = tpbbl.BrandId
                group by
	                s.Name,
	                rd.FileInfoId,
	                fi.Path,
	                rd.TargetPharmacyId,
                    rd.RegionCode, 
                    rd.RegionShortName,
	                {2}
            ";

            query = string.Format(query,
                CreateInternalFilter(searchRawDataByClassifierModel),
                CreateAdditionalExternalFields(searchRawDataByClassifierModel),
                CreateGroupByFields(searchRawDataByClassifierModel)
                );

            using (var context = new RetailContext(APP))
            {
                List<AggregatedRawDataByClassifier> result = await context.Database.SqlQuery<AggregatedRawDataByClassifier>(query).ToListAsync();
                ProcessData(result);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        #region Создание полей результирующих данных

        private static string CreateAdditionalExternalFields(SearchRawDataByClassifierModel searchModel)
        {
            var fields = new List<string>
            {
                CreateGroupName(searchModel.DetailingType),
                CreateFieldsByPeriodDetailingType(searchModel),
            };

            return string.Join(", ", fields.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        private static string CreateGroupName(DetailingType detailingType)
        {
            switch (detailingType)
            {
                case DetailingType.Brand:
                    return "ev.Brand as Name";
                case DetailingType.TradeName:
                    return "ev.TradeName as Name";
                case DetailingType.Dop:
                    return "ev.TradeName + ' ' + ev.DrugDescription + ' ' + ev.OwnerTradeMark + ' ' + ev.Packer as Name";
                default:
                    throw new ArgumentException(string.Format("DetailingType '{0}' is not supported", detailingType));
            }
        }

        #endregion


        #region Создание полей для группировки

        private static string CreateGroupByFields(SearchRawDataByClassifierModel searchModel)
        {
            var fields = new List<string>
            {
                CreateGroupByDetailingType(searchModel),
                CreateFieldsByPeriodDetailingType(searchModel),
            };

            return string.Join(", ", fields.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        private static string CreateGroupByDetailingType(SearchRawDataByClassifierModel searchModel)
        {
            switch (searchModel.DetailingType)
            {
                case DetailingType.Brand:
                    return "ev.BrandId, ev.Brand";
                case DetailingType.TradeName:
                    return "ev.TradeNameId, ev.TradeName";
                case DetailingType.Dop:
                    return "ev.TradeNameId, ev.TradeName, ev.DrugDescription, ev.OwnerTradeMark, ev.OwnerTradeMarkId, ev.PackerId, ev.Packer";
                default:
                    throw new ArgumentException(string.Format("DetailingType '{0}' is not supported", searchModel.DetailingType));
            }
        }

        #endregion



        private static string CreateFieldsByPeriodDetailingType(SearchRawDataByClassifierModel searchModel)
        {
            switch (searchModel.PeriodDetailingType)
            {
                case PeriodDetailingType.None:
                    return string.Empty;
                case PeriodDetailingType.Year:
                    return "rd.Year";
                case PeriodDetailingType.Month:
                    return "rd.Year, rd.Month";
                default:
                    throw new ArgumentException(string.Format("PeriodDetailingType '{0}' is not supported", searchModel.PeriodDetailingType));
            }
        }


        private static string CreateInternalFilter(SearchRawDataByClassifierModel searchModel)
        {
            var filter = new StringBuilder(CreatePeriodCondition(searchModel));

            if (!IsArrayEmpty(searchModel.RegionCodes))
            {
                string regionCodesAsString = string.Join(", ", searchModel.RegionCodes.Select(s => string.Format("'{0}'", s)));
                filter.AppendFormat("and r.Code in ({0}) -- Фильтр по региону", regionCodesAsString);
                filter.AppendLine();
            }

            if (!IsArrayEmpty(searchModel.TradeNameIds))
            {
                filter.AppendFormat("and ev.TradeNameId in ({0}) -- Фильтр по TradeName", JoinIds(searchModel.TradeNameIds));
                filter.AppendLine();
            }

            if (!IsArrayEmpty(searchModel.BrandIds))
            {
                filter.AppendFormat("and ev.BrandId in ({0}) -- Фильтр по бренду", JoinIds(searchModel.BrandIds));
                filter.AppendLine();
            }

            if (!IsArrayEmpty(searchModel.ClassifierIds))
            {
                filter.AppendFormat("and ev.ClassifierId in ({0}) -- Фильтр по DOP", JoinIds(searchModel.ClassifierIds));
                filter.AppendLine();
            }

            return filter.ToString();
        }

        private static bool IsArrayEmpty<T>(T[] array)
        {
            return !(array != null && array.Length > 0);
        }

        private static string JoinIds(int[] array)
        {
            return string.Join(", ", array.Select(s => string.Format("{0}", s)));
        }

        private static string CreatePeriodCondition(SearchRawDataByClassifierModel searchModel)
        {
            var filter = new StringBuilder();
            filter.AppendFormat("({0} > {2} or ({0} = {2} and {1} >= {3})) and ({0} < {4} or ({0} = {4} and {1} <= {5})) -- Объединение по периоду",
                "rd.Year",
                "rd.Month",

                searchModel.YearStart,
                searchModel.MonthStart,

                searchModel.YearEnd,
                searchModel.MonthEnd
                );

            filter.AppendLine();

            return filter.ToString();
        }

        private static void ProcessData(IEnumerable<AggregatedRawDataByClassifier> items)
        {
            const string pathPrefix = @"\\gk.bionika.ru\MSK\HQ\Alpharm\ДРА\Main\RetailData\Current\";

            foreach (AggregatedRawDataByClassifier item in items)
            {
                item.Path = pathPrefix + item.Path;
                item.PurchasePriceNds = item.PurchaseCount.HasValue ? item.PurchaseSumNds / item.PurchaseCount : null;
                item.SellingPriceNds = item.SellingCount.HasValue ? item.SellingSumNds / item.SellingCount : null;
            }
        }
    }
}