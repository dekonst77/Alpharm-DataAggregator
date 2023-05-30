using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    [Table("GoodsSystematizationView", Schema = "GoodsClassifier")]
    public class GoodsSystematizationView
    {
        [Key]
        public long ClassifierId { get; set; }
        public long GoodsId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public long GoodsTradeNameId { get; set; }
        public long? GoodsCategoryId { get; set; }

        public string GoodsTradeName { get; set; }
        public string GoodsDescription { get; set; }

        public string OwnerTradeMark { get; set; }

        public string Packer { get; set; }

        public string GoodsCategoryName { get; set; }
        public bool Used { get; set; }
        public bool? ToRetail { get; set; }
        public string Comment { get; set; }
        public string ProductInfoComment { get; set; }
    }
}