using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsCategoryKeyword", Schema = "GoodsSystematization")]
    public class GoodsCategoryKeyword
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long GoodsCategoryId { get; set; }
    }
}