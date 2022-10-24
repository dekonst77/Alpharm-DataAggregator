using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("Equipment", Schema = "Classifier")]
    public class Equipment : Common.DictionaryItem
    {

    }
}