using System.Collections.Generic;
using DataAggregator.Web.Models.Retail.CommonPriceRuleEditor;

namespace DataAggregator.Web.Models.Retail.PriceRuleEditor
{
    public class PriceRuleModel
    {
        public long? Id { get; set; }
        public long? PriceRuleId { get; set; }
        public int? ClassifierId { get; set; }
        public string RegionCode { get; set; }
        public List<PriceRuleRegionModel> Regions { get; set; } 
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public decimal? PurchasePriceMin { get; set; }
        public decimal? PurchasePriceMax { get; set; }
        public string Comment { get; set; }
    }
}