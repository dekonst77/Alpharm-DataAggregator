namespace DataAggregator.Web.Models.Retail.GoodsCountRuleEditor
{
    public class GoodsCountRuleData
    {
        public long GoodsId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public long? BrandId { get; set; }
        public string Brand { get; set; }
        public string GoodsDescription { get; set; }

        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string GoodsTradeName { get; set; }

        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public decimal? SellingSumNDS { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? SellingSumNDSPart { get; set; }
        public decimal? SellingCountPart { get; set; }

        public bool IsInCountRules { get; set; }

        public long ClassifierId { get; set; }
    }
}