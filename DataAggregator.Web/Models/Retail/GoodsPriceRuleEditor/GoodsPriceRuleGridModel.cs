namespace DataAggregator.Web.Models.Retail.GoodsPriceRuleEditor
{
    public class GoodsPriceRuleGridModel
    {
        public long GoodsId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public string GoodsDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
    }
}