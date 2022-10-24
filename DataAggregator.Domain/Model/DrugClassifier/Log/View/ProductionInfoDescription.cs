using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Log.View
{   

    [Table("ProductionInfoDescription", Schema = "Log")]
    public class ProductionInfoDescription
    {
        public long Id { get; set; }
        public string Description { get; set; }
    }
}
