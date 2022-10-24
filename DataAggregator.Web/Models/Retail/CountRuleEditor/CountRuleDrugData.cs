namespace DataAggregator.Web.Models.Retail.CountRuleEditor
{
    public class CountRuleDrugData
    {
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public long BrandId { get; set; }
        public string Brand { get; set; }
        public string DrugDescription { get; set; }

        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string TradeName { get; set; }

        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public decimal? SellingSumNDS { get; set; }
        public decimal? PurchaseSumNDS { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? PurchaseCount { get; set; }
        public decimal? SellingSumNDSPart { get; set; }
        public decimal? PurchaseSumNDSPart { get; set; }
        public decimal? SellingCountPart { get; set; }
        public decimal? PurchaseCountPart { get; set; }

        public bool IsInCountRules { get; set; }

        public long ClassifierId { get; set; }
    }
}