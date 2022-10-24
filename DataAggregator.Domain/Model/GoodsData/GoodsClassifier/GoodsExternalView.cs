using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsExternalView", Schema = "GoodsClassifier")]
    public class GoodsExternalView
    {
        [Key]
        public long ClassifierId { get; set; }
        public long? GoodsId { get; set; }
        public long? GoodsTradeNameId { get; set; }
        public string GoodsTradeName { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? PackerId { get; set; }
        public string Packer { get; set; }
        public string GoodsDescription { get; set; }
        public long? BrandId { get; set; }
        public string Brand { get; set; }
    }
}