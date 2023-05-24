using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("Goods", Schema = "GoodsClassifier")]
    public class Goods
    {
        public long Id { get; set; }

        public string GoodsDescription { get; set; }

        public string GoodsDescription_Eng { get; set; }

        public long GoodsTradeNameId { get; set; }

        public virtual GoodsTradeName GoodsTradeName { get; set; }

        public virtual List<GoodsProductionInfo> GoodsProductionInfo { get; set; }
    }
}