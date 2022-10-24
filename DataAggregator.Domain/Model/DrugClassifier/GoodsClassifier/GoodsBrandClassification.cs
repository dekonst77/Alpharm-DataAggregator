using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsBrandClassification", Schema = "GoodsClassifier")]
    public class GoodsBrandClassification
    {
        public long Id { get; set; }

        public long GoodsTradeNameId { get; set; }
        
        public long OwnerTradeMarkId { get; set; }

        public long BrandId { get; set; }

        public virtual GoodsTradeName GoodsTradeName { get; set; }
        public virtual Manufacturer OwnerTradeMark { get; set; }
        public virtual Brand GoodsBrand { get; set; }
    }
}