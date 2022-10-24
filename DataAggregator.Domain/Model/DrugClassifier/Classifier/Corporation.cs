using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("Corporation", Schema = "Classifier")]
    public class Corporation : Common.DictionaryItem
    {
        public string Value_eng { get; set; }
    }
}
