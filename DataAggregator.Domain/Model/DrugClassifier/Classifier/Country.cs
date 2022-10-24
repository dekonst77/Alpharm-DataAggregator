using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("Country", Schema = "Classifier")]
    public class Country : Common.DictionaryItem
    {
        public long? LocalizationId { get; set; }
        public virtual Localization Localization { get; set; }
    }
}
