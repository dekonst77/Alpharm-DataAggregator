using DataAggregator.Domain.Model.GoodsData;
using System;

namespace DataAggregator.Web.Models.Retail.GoodsCountRuleEditor
{
    public class GoodsCountRuleModel
    {
        public long? Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public string Region { get; set; }

        public long? GoodsId { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public long? PackerId { get; set; }
        public string FullGoodsDescription { get; set; }
        public long? ClassifierId { get; set; }

        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string GoodsTradeName { get; set; }

        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public DateTime Date { get; set; }

        public long? DistributionGoodsId { get; set; }
        public long? DistributionOwnerTradeMarkId { get; set; }
        public long? DistributionPackerId { get; set; }
        public string DistributionFullGoodsDescription { get; set; }
        public long? DistributionClassifierId { get; set; }

        /// <summary>
        /// Торговое наименование - куда распределили
        /// </summary>
        public string DistributionGoodsTradeName { get; set; }    

        public int? SellingSumPart { get; set; }
        public int? TopCountFrom { get; set; }
        public int? TopCountTo { get; set; }
        public bool RegionMsk { get; set; }
        public bool RegionSpb { get; set; }
        public bool RegionRus { get; set; }

        public GoodsCountRuleModel()
        {
        }

        public static GoodsCountRuleModel Create(GoodsCountRuleView model)
        {
            return ModelMapper.Mapper.Map<GoodsCountRuleModel>(model);
        }

        /// <summary>
        /// Заполним model 
        /// </summary>
        public void SetGoodsCountRule(GoodsCountRule model, Guid userId)
        {

            model.Year = Year;
            model.Month = Month;
            model.RegionCode = RegionCode;
            model.GoodsId = GoodsId;
            model.OwnerTradeMarkId = OwnerTradeMarkId;
            model.PackerId = PackerId;
            model.Date = DateTime.Now;
            model.UserId = userId;
            model.DistributionGoodsId = DistributionGoodsId;
            model.DistributionOwnerTradeMarkId = DistributionOwnerTradeMarkId;
            model.DistributionPackerId = DistributionPackerId;
            model.SellingSumPart = SellingSumPart ?? 0;
            model.TopCountFrom = TopCountFrom == 0 ? null : TopCountFrom;
            model.TopCountTo = TopCountTo == 0 ? null : TopCountTo;
            model.RegionMsk = RegionMsk;
            model.RegionSpb = RegionSpb;
            model.RegionRus = RegionRus;
        }
    }
}