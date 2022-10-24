using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using DataAggregator.Domain.Model.GoodsData;
using DataAggregator.Domain.Model.GoodsData.QueryModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;

namespace DataAggregator.Domain.DAL
{
    public class GoodsDataContext : DbContext
    {
        #region GoodsClassifier

        public DbSet<GoodsExternalView> GoodsExternalView { get; set; }

        #endregion

        public DbSet<CalcInfo> CalcInfo { get; set; }
        
        public DbSet<GoodsPriceRuleListView> GoodsPriceRuleListView { get; set; }

        public DbSet<GoodsPriceRule> GoodsPriceRule { get; set; }
        public DbSet<GoodsCountRule> GoodsCountRule { get; set; }

        public DbSet<GoodsCountRuleView> GoodsCountRuleView { get; set; }

        public DbSet<GoodsCountRuleFullVolume> GoodsCountRuleFullVolume { get; set; }

        public DbSet<GoodsCountRuleFullVolumeView> GoodsCountRuleFullVolumeView { get; set; }

        public DbSet<RegionPM12View> RegionPM12View { get; set; }

        public GoodsDataContext(string APP)
        {
            Database.SetInitializer<GoodsDataContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
            Database.CommandTimeout = 6000;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public List<GoodsCalcRuleModel> SearchGoodsPriceRuleModel(long goodsId, long ownerTradeMarkId, long packerId, int year, int month)
        {
            var query = @"  select 
                                CAST(t.GoodsId as bigint) as GoodsId, 
                                CAST(t.OwnerTradeMarkId as bigint) as OwnerTradeMarkId, 
                                CAST(t.PackerId as bigint) as PackerId, 
                                gev.BrandId,
                                gev.Brand,
                                gev.GoodsTradeName + ' ' + gev.GoodsDescription as GoodsDescription, 
                                gev.GoodsTradeName, 
                                gev.OwnerTradeMark as OwnerTradeMark, 
                                gev.Packer as Packer,
                                null as SellingSumNDS,
                                null as SellingCount,
                                null as SellingSumNDSPart,
                                null as SellingCountPart,
                                cast(0 as bit) as IsInCountRules
                            from
                            (
                               select rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId
                               from calc.Region0DataView as rd
                               where rd.Year = " + year + " and rd.Month = " + month + @" group by rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId
                            ) as t
                            inner join GoodsClassifier.GoodsExternalView AS gev
                                on gev.GoodsId = t.GoodsId and gev.OwnerTradeMarkId = t.OwnerTradeMarkId and gev.PackerId = t.PackerId 
                            where t.GoodsId = " + goodsId + " and t.OwnerTradeMarkId = " + ownerTradeMarkId + " and t.PackerId = " + packerId;

            return Database.SqlQuery<GoodsCalcRuleModel>(query).ToList();
        }

        public List<GoodsCalcRuleModel> SearchGoodsPriceRuleModel(string value, int year, int month)
        {
            string query = @"  select 
                                CAST(t.GoodsId as bigint) as GoodsId, 
                                CAST(t.OwnerTradeMarkId as bigint) as OwnerTradeMarkId, 
                                CAST(t.PackerId as bigint) as PackerId, 
                                gev.BrandId,
                                gev.Brand,
                                gev.GoodsTradeName + ' ' + gev.GoodsDescription as GoodsDescription, 
                                gev.GoodsTradeName, 
                                gev.OwnerTradeMark as OwnerTradeMark, 
                                gev.Packer as Packer,
                                null as SellingSumNDS,
                                null as SellingCount,
                                null as SellingSumNDSPart,
                                null as SellingCountPart,
                                cast(0 as bit) as IsInCountRules
                            from
                            (
                             select rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId
                             from calc.Region0DataView as rd
                             where rd.Year = " + year + " and rd.Month = " + month + @" group by rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId
                            ) as t
                            inner join GoodsClassifier.GoodsExternalView AS gev
                                on gev.GoodsId = t.GoodsId and gev.OwnerTradeMarkId = t.OwnerTradeMarkId and gev.PackerId = t.PackerId 
                            where gev.GoodsTradeName + ' ' + gev.GoodsDescription + ' ' + gev.OwnerTradeMark + ' ' + gev.Packer like '%" + value + "%'";

            return Database.SqlQuery<GoodsCalcRuleModel>(query).ToList();
        }

        public async Task<List<RegionPM12View>> SearchRegionAsync(params string[] values)
        {
            return await RegionPM12View.Where(s => values.Any(v => (s.RegionPM12 + " " + s.FullName).Contains(v))).ToListAsync();
        }

        public bool? CheckDataForAvgPrice(int year, int month, string regionCode, bool msk, bool spb, bool rf, long goodsId, long ownerTradeMarkId, long packerId)
        {
            var query = @"
                declare @year int = " + year + @";
                declare @month int = " + month + @";
                declare @regionCode nvarchar(10) = '" + regionCode + @"';
                declare @msk bit = " + (msk ? 1 : 0) + @";
                declare @spb bit = " + (spb ? 1 : 0) + @";
                declare @rf bit = " + (rf ? 1 : 0) + @";
                declare @goodsId bigint = " + goodsId + @";
                declare @ownerTradeMarkId bigint = " + ownerTradeMarkId + @";
                declare @packerId bigint = " + packerId + @";

                select cast(iif(sum(SellingSumNDS) is null, 0, 1) as bit) as HasSellingData
                from calc.Region12DataView as rd
                inner join 
                (
                  select ci.Year, ci.Month
                  from calc.CalcInfo as ci
                  where (@year-ci.Year)*12 + (@month-ci.Month) <= 6
                    and (@year-ci.Year)*12 + (@month-ci.Month) > 0
                ) as ml on ml.Year-2000 = rd.Year and ml.Month = rd.Month
                where rd.GoodsId = @goodsId and rd.OwnerTradeMarkId = @ownerTradeMarkId and rd.PackerId = @packerId
                and (rd.RegionCode = @regionCode or @msk = 1 and rd.RegionCode like '77.%' or @spb = 1 and rd.RegionCode like '78.%' or @rf = 1)
            ";

            return Database.SqlQuery<bool>(query).FirstOrDefault();
        }

        public List<GoodsCalcRuleModel> SearchGoodsInR12CountRuleModel(long brandId, int year, int month, string regionCode)
        {
            var query = @"
                select 
                gop.GoodsId,
                gop.OwnerTradeMarkId,
                gop.PackerId,
                gop.BrandId,
                gevAll.Brand,
                gevAll.GoodsTradeName + ' ' + gevAll.GoodsDescription as GoodsDescription, 
                gevAll.GoodsTradeName, 
                gevAll.OwnerTradeMark as OwnerTradeMark, 
                gevAll.Packer as Packer,
                gop.SellingSumNDS,
                gop.SellingCount,
                iif(gop.SellingSumNDS > 0, 100 * gop.SellingSumNDS/brand.SellingSumNDS, 0) as SellingSumNDSPart,
                iif(gop.SellingCount > 0, 100 * gop.SellingCount/brand.SellingCount, 0) as SellingCountPart,
                cast(case when cr.GoodsId is null then 0 else 1 end as bit) as IsInCountRules,
                gevAll.ClassifierId
                from
                (
	                select 
	                  cast(rd.GoodsId as bigint) as GoodsId, 
	                  cast(rd.OwnerTradeMarkId as bigint) as OwnerTradeMarkId,
	                  cast(rd.PackerId as bigint) as PackerId,
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region12DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
                    where gev.BrandId = " + brandId + @" and rd.RegionCode = '" + regionCode + @"' and rd.Year = " + year + @" and rd.Month = " + month + @" 
	                group by rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId, gev.BrandId
                ) as gop
                inner join
                (
	                select 
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region12DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
	                where gev.BrandId = " + brandId + @" and rd.RegionCode = '" + regionCode + @"' and rd.Year = " + year + @" and rd.Month = " + month + @" 
	                group by gev.BrandId
                ) as brand on gop.BrandId = brand.BrandId
                inner join GoodsClassifier.GoodsExternalView AS gevAll on gevAll.GoodsId = gop.GoodsId and gevAll.OwnerTradeMarkId = gop.OwnerTradeMarkId and gevAll.PackerId = gop.PackerId 
				left outer join
				(
					select
						cr.GoodsId,
						cr.OwnerTradeMarkId,
						cr.PackerId
					from calc.GoodsCountRule as cr
					where cr.Year = 2000 + " + year + @" and cr.Month = " + month + @" and cr.RegionCode = '" + regionCode + @"'
					group by cr.GoodsId, cr.OwnerTradeMarkId, cr.PackerId
				) as cr
				on cr.GoodsId = gop.GoodsId and cr.OwnerTradeMarkId = gop.OwnerTradeMarkId and cr.PackerId = gop.PackerId
            ";

            return Database.SqlQuery<GoodsCalcRuleModel>(query).ToList();
        }

        public List<GoodsCalcRuleModel> SearchGoodsInR1CountRuleModel(long brandId, int year, int month, string regionCode)
        {
            string regionCondition;

            switch (regionCode)
            {
                case "77":
                    regionCondition = "cr.RegionMsk = 1";
                    break;
                case "78":
                    regionCondition = "cr.RegionSpb = 1";
                    break;
                default:
                    throw new ApplicationException("incorrect region code");
            }

            var query = @"
                select 
                gop.GoodsId,
                gop.OwnerTradeMarkId,
                gop.PackerId,
                gop.BrandId,
                gevAll.Brand,
                gevAll.GoodsTradeName + ' ' + gevAll.GoodsDescription as GoodsDescription, 
                gevAll.GoodsTradeName, 
                gevAll.OwnerTradeMark as OwnerTradeMark, 
                gevAll.Packer as Packer,
                gop.SellingSumNDS,
                gop.SellingCount,
                iif(gop.SellingSumNDS > 0, 100 * gop.SellingSumNDS/brand.SellingSumNDS, 0) as SellingSumNDSPart,
                iif(gop.SellingCount > 0, 100 * gop.SellingCount/brand.SellingCount, 0) as SellingCountPart,
                cast(case when cr.GoodsId is null then 0 else 1 end as bit) as IsInCountRules,
                gevAll.ClassifierId
                from
                (
	                select 
	                  cast(rd.GoodsId as bigint) as GoodsId, 
	                  cast(rd.OwnerTradeMarkId as bigint) as OwnerTradeMarkId,
	                  cast(rd.PackerId as bigint) as PackerId,
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region1DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
	                where gev.BrandId = " + brandId + @" and rd.RegionCode = '" + regionCode + @"' and rd.Year = " + year + @" and rd.Month = " + month + @" 
	                group by rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId, gev.BrandId
                ) as gop
                inner join
                (
	                select 
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region1DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
	                where gev.BrandId = " + brandId + @" and rd.RegionCode = '" + regionCode + @"' and rd.Year = " + year + @" and rd.Month = " + month + @" 
	                group by gev.BrandId
                ) as brand on gop.BrandId = brand.BrandId
                inner join GoodsClassifier.GoodsExternalView AS gevAll on gevAll.GoodsId = gop.GoodsId and gevAll.OwnerTradeMarkId = gop.OwnerTradeMarkId and gevAll.PackerId = gop.PackerId 
                left outer join
				(
					select
						cr.GoodsId,
						cr.OwnerTradeMarkId,
						cr.PackerId
					from calc.GoodsCountRule as cr
					where cr.Year = 2000 + " + year + @" and cr.Month = " + month + @" and " + regionCondition + @"
					group by cr.GoodsId, cr.OwnerTradeMarkId, cr.PackerId
				) as cr
				on cr.GoodsId = gop.GoodsId and cr.OwnerTradeMarkId = gop.OwnerTradeMarkId and cr.PackerId = gop.PackerId
            ";

            return Database.SqlQuery<GoodsCalcRuleModel>(query).ToList();
        }

        public List<GoodsCalcRuleModel> SearchGoodsInRusCountRuleModel(long brandId, int year, int month)
        {
            var query = @"
                select 
                gop.GoodsId,
                gop.OwnerTradeMarkId,
                gop.PackerId,
                gop.BrandId,
                gevAll.Brand,
                gevAll.GoodsTradeName + ' ' + gevAll.GoodsDescription as GoodsDescription, 
                gevAll.GoodsTradeName, 
                gevAll.OwnerTradeMark as OwnerTradeMark, 
                gevAll.Packer as Packer,
                gop.SellingSumNDS,
                gop.SellingCount,
                iif(gop.SellingSumNDS > 0, 100 * gop.SellingSumNDS/brand.SellingSumNDS, 0) as SellingSumNDSPart,
                iif(gop.SellingCount > 0, 100 * gop.SellingCount/brand.SellingCount, 0) as SellingCountPart,
                cast(case when cr.GoodsId is null then 0 else 1 end as bit) as IsInCountRules,
                gevAll.ClassifierId
                from
                (
	                select 
	                  cast(rd.GoodsId as bigint) as GoodsId, 
	                  cast(rd.OwnerTradeMarkId as bigint) as OwnerTradeMarkId,
	                  cast(rd.PackerId as bigint) as PackerId,
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region0DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
	                where gev.BrandId = " + brandId + @" and rd.Year = " + year + @" and rd.Month = " + month + @" 
	                group by rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId, gev.BrandId
                ) as gop
                inner join
                (
	                select 
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region0DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
	                where gev.BrandId = " + brandId + @" and rd.Year = " + year + @" and rd.Month = " + month + @" 
	                group by gev.BrandId
                ) as brand on gop.BrandId = brand.BrandId
                inner join GoodsClassifier.GoodsExternalView AS gevAll on gevAll.GoodsId = gop.GoodsId and gevAll.OwnerTradeMarkId = gop.OwnerTradeMarkId and gevAll.PackerId = gop.PackerId 
				left outer join
				(
					select
						cr.GoodsId,
						cr.OwnerTradeMarkId,
						cr.PackerId
					from calc.GoodsCountRule as cr
					where cr.Year = 2000 + " + year + @" and cr.Month = " + month + @" and cr.RegionRus = 1
					group by cr.GoodsId, cr.OwnerTradeMarkId, cr.PackerId
				) as cr
				on cr.GoodsId = gop.GoodsId and cr.OwnerTradeMarkId = gop.OwnerTradeMarkId and cr.PackerId = gop.PackerId
            ";

            return Database.SqlQuery<GoodsCalcRuleModel>(query).ToList();
        }

        public List<GoodsCalcRuleModel> SearchGoodsInRusCountRuleFullVolumeModel(long brandId)
        {
            string query = @"
                select 
                gop.GoodsId,
                gop.OwnerTradeMarkId,
                gop.PackerId,
                gop.BrandId,
                gevAll.Brand,
                gevAll.GoodsTradeName + ' ' + gevAll.GoodsDescription as GoodsDescription, 
                gevAll.GoodsTradeName, 
                gevAll.OwnerTradeMark as OwnerTradeMark, 
                gevAll.Packer as Packer,
                gop.SellingSumNDS,
                gop.SellingCount,
                iif(gop.SellingSumNDS > 0, 100 * gop.SellingSumNDS/brand.SellingSumNDS, 0) as SellingSumNDSPart,
                iif(gop.SellingCount > 0, 100 * gop.SellingCount/brand.SellingCount, 0) as SellingCountPart,
                cast(case when cr.GoodsId is null then 0 else 1 end as bit) as IsInCountRules,
                gevAll.ClassifierId
                from
                (
	                select 
	                  cast(rd.GoodsId as bigint) as GoodsId, 
	                  cast(rd.OwnerTradeMarkId as bigint) as OwnerTradeMarkId,
	                  cast(rd.PackerId as bigint) as PackerId,
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region0DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
	                where gev.BrandId = " + brandId + @" 
	                group by rd.GoodsId, rd.OwnerTradeMarkId, rd.PackerId, gev.BrandId
                ) as gop
                inner join
                (
	                select 
	                  gev.BrandId,
	                  sum(SellingSumNDS)  as SellingSumNDS,
	                  sum(SellingCount)   as SellingCount
	                from calc.Region0DataView as rd
	                inner join GoodsClassifier.GoodsExternalView AS gev on gev.GoodsId = rd.GoodsId and gev.OwnerTradeMarkId = rd.OwnerTradeMarkId and gev.PackerId = rd.PackerId 
	                where gev.BrandId = " + brandId + @" 
	                group by gev.BrandId
                ) as brand on gop.BrandId = brand.BrandId
                inner join GoodsClassifier.GoodsExternalView AS gevAll on gevAll.GoodsId = gop.GoodsId and gevAll.OwnerTradeMarkId = gop.OwnerTradeMarkId and gevAll.PackerId = gop.PackerId 
				left outer join
				(
					select
						cr.GoodsId,
						cr.OwnerTradeMarkId,
						cr.PackerId
					from calc.GoodsCountRuleFullVolume as cr
					group by cr.GoodsId, cr.OwnerTradeMarkId, cr.PackerId
				) as cr
                on cr.GoodsId = gop.GoodsId and cr.OwnerTradeMarkId = gop.OwnerTradeMarkId and cr.PackerId = gop.PackerId
            ";

            return Database.SqlQuery<GoodsCalcRuleModel>(query).ToList();
        }
    }
}