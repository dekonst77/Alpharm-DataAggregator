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
    public sealed class SearchRawDataByGoodsClassifierController : BaseController
    {
        /// <summary>
        /// �������� ������
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
			                rd.FileInfoId,
							ev.ClassifierId, sp.Id as SourcePharmacyId, sp.TargetPharmacyId, r.Code as RegionCode, r.FullName as RegionShortName,
							c.GoodsId,
							c.OwnerTradeMarkId,
							c.PackerId,
			                sum(rd.PurchaseSumNDS) as PurchaseSumNDS,
			                sum(rd.SellingSumNDS) as SellingSumNDS,
			                sum(rd.PurchaseCount) as PurchaseCount,
			                sum(rd.SellingCount) as SellingCount,
							max(case when spr.SourcePharmacyId is null then 0 else 1 end) as IsSprBlackList
		                from dbo.RawData as rd
		                left outer join Retail.SourcePharmacy as sp
			                on sp.Id = rd.SourcePharmacyId
		                inner join GoodsSystematization.GoodsClassifier as c
			                on c.Id = rd.GoodsClassifierId
						inner join GoodsClassifier.GoodsExternalView as ev
							on c.GoodsId = ev.GoodsId and
							   c.OwnerTradeMarkId = ev.OwnerTradeMarkId and
							   c.PackerId = ev.PackerId
		                left outer join Retail.TargetPharmacy as tp
			                on sp.TargetPharmacyId = tp.Id
		                inner join Retail.Region as r
			                on r.Code = tp.RegionPM12
                        left join Retail.SourcePharmacyRelevance spr
							on rd.SourcePharmacyId = spr.SourcePharmacyId and rd.Year = spr.Year and rd.Month = spr.Month
		                where
			                {0}
		                group by
			                rd.Year, rd.Month, rd.FileInfoId, ev.ClassifierId, sp.Id, sp.TargetPharmacyId, r.Code, r.FullName, c.GoodsId, c.OwnerTradeMarkId, c.PackerId
	                )
                select top 50000
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
                inner join Retail.FileInfo as fi
	                on fi.Id = rd.FileInfoId
                inner join Retail.Source as s
	                on s.Id = fi.SourceId
				inner join GoodsClassifier.GoodsExternalView as ev
					on rd.GoodsId = ev.GoodsId and
		  			   rd.OwnerTradeMarkId = ev.OwnerTradeMarkId and
					   rd.PackerId = ev.PackerId
                left join Retail.TargetPharmacyBlackList tpbl
                    on
                        rd.TargetPharmacyId = tpbl.TargetPharmacyId and rd.Year = tpbl.Year and rd.Month = tpbl.Month
                left join Retail.TargetPharmacyBrandBlackList as tpbbl
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

            using (var context = new GoodsDataContext(APP))
            {
                List<AggregatedRawDataByClassifier> result =
                    await context.Database.SqlQuery<AggregatedRawDataByClassifier>(query).ToListAsync();
                ProcessData(result);

                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        
        private static string CreateInternalFilter(SearchRawDataByClassifierModel searchModel)
        {
            var filter = new StringBuilder(CreatePeriodCondition(searchModel));

            if (!IsArrayEmpty(searchModel.RegionCodes))
            {
                string regionCodesAsString = string.Join(", ", searchModel.RegionCodes.Select(s => string.Format("'{0}'", s)));
                filter.AppendFormat("and r.Code in ({0}) -- ������ �� �������", regionCodesAsString);
                filter.AppendLine();
            }

            if (!IsArrayEmpty(searchModel.TradeNameIds))
            {
                filter.AppendFormat("and ev.GoodsTradeNameId in ({0}) -- ������ �� TradeName", JoinIds(searchModel.TradeNameIds));
                filter.AppendLine();
            }

            if (!IsArrayEmpty(searchModel.BrandIds))
            {
                filter.AppendFormat("and ev.BrandId in ({0}) -- ������ �� ������", JoinIds(searchModel.BrandIds));
                filter.AppendLine();
            }

            if (!IsArrayEmpty(searchModel.ClassifierIds))
            {
                filter.AppendFormat("and ev.ClassifierId in ({0}) -- ������ �� DOP", JoinIds(searchModel.ClassifierIds));
                filter.AppendLine();
            }

            return filter.ToString();
        }

        #region �������� ����� �������������� ������

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
                    return "ev.GoodsTradeName as Name";
                case DetailingType.Dop:
                    return "ev.GoodsTradeName + ' ' + ev.GoodsDescription + ' ' + ev.OwnerTradeMark + ' ' + ev.Packer + ' ' + ev.Brand as Name";
                default:
                    throw new ArgumentException(string.Format("DetailingType '{0}' is not supported", detailingType));
            }
        }

        #endregion


        #region �������� ����� ��� �����������

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
                    return "ev.GoodsTradeNameId, ev.GoodsTradeName";
                case DetailingType.Dop:
                    return "ev.GoodsTradeNameId, ev.GoodsTradeName, ev.GoodsDescription, ev.OwnerTradeMark, ev.OwnerTradeMarkId, ev.PackerId, ev.Packer, ev.Brand";
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
            filter.AppendFormat("({0} > {2} or ({0} = {2} and {1} >= {3})) and ({0} < {4} or ({0} = {4} and {1} <= {5})) -- ����������� �� �������",
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
            const string pathPrefix = @"\\gk.bionika.ru\MSK\HQ\Alpharm\���\Main\RetailData\Current\";

            foreach (AggregatedRawDataByClassifier item in items)
            {
                item.Path = pathPrefix + item.Path;
                item.PurchasePriceNds = item.PurchaseCount.HasValue ? item.PurchaseSumNds / item.PurchaseCount : null;
                item.SellingPriceNds = item.SellingCount.HasValue ? item.SellingSumNds / item.SellingCount : null;
            }
        }
    }
}