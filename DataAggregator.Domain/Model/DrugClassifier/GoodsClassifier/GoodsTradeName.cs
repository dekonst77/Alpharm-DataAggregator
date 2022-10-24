using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsTradeName", Schema = "GoodsClassifier")]
    public class GoodsTradeName : Common.DictionaryItem
    {
        public string ValueEnglish { get; set; }
    }
}