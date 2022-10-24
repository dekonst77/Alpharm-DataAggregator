using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsType", Schema = "GoodsClassifier")]
    public class GoodsType : Common.DictionaryItem
    {
    }
}