
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{

    /// <summary>
    /// Стадии производства
    /// </summary>
    [Table("ProductionStage", Schema = "Classifier")]
    public class ProductionStage : Common.DictionaryItem
    {
    }


}