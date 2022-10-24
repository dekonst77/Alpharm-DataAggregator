using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Log.View
{
    [Table("GoodsProductionInfoDescription", Schema = "Log")]
    public class GoodsProductionInfoDescription
    {
        public long Id { get; set; }
        public string Description { get; set; }
    }
}
