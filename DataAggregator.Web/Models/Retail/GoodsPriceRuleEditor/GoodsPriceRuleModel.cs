using System.Collections.Generic;
using DataAggregator.Web.Models.Retail.CommonPriceRuleEditor;

namespace DataAggregator.Web.Models.Retail.GoodsPriceRuleEditor
{
    public class GoodsPriceRuleModel
    {
        public long? Id { get; set; }
        public long? PriceRuleId { get; set; }
        public long? GoodsId { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public long? PackerId { get; set; }
        public string RegionCode { get; set; }
        public List<PriceRuleRegionModel> Regions { get; set; } 
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public string Comment { get; set; }
    }
}