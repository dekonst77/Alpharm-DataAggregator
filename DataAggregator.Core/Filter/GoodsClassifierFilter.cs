using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;

namespace DataAggregator.Core.Filter
{
    public class GoodsClassifierFilter
    {
        public long? ClassifierId { get; set; }
        public long? GoodsId { get; set; }
        public string GoodsDescription { get; set; }
        public long? GoodsDescriptionId { get; set; }
        public string GoodsTradeName { get; set; }
        public long? GoodsTradeNameId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string Packer { get; set; }
        public long? PackerId { get; set; }
        public GoodsCategory GoodsCategory { get; set; }
        public bool Used { get; set; }
        public bool ToRetail { get; set; }
    }
}