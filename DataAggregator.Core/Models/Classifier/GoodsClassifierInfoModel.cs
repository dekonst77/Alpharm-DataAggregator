using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;

namespace DataAggregator.Core.Models.Classifier
{
    public class GoodsClassifierInfoModel
    {
        public long GoodsId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }

        public string GoodKey { get; set; }

        public string GoodsDescription { get; set; }
        public GoodsTradeName GoodsTradeName { get; set; }
        public Brand GoodsBrand { get; set; }
    }
}