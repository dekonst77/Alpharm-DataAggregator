using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierRxOtc
{
    [Table("ClassifierRxOtcLog", Schema = "log")]
    public class ClassifierRxOtcLog
    {
        [Key]
        public long Id { get; set; }
        public long ClassifierId { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public System.DateTime When { get; set; }
        public string Flag { get; set; }
    }
}
