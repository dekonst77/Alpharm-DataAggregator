using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsProductionInfo", Schema = "GoodsClassifier")]
    public class GoodsProductionInfo
    {
        public long Id { get; set; }

        public long GoodsId { get; set; }
        
        public long OwnerTradeMarkId { get; set; }

        public long PackerId { get; set; }

        public long? GoodsCategoryId { get; set; }

        public virtual Manufacturer OwnerTradeMark { get; set; }
        public virtual Manufacturer Packer { get; set; }
        public virtual GoodsCategory GoodsCategory { get; set; }
        public virtual Goods Goods { get; set; }
        public bool Used { get; set; }
        public string Comment { get; set; }
        public GoodsProductionInfo Copy()
        {
            return new GoodsProductionInfo
            {
                OwnerTradeMarkId = this.OwnerTradeMarkId,
                PackerId = this.PackerId,
                GoodsId = this.GoodsId,
                Id = this.Id

            };
        }
    }
}