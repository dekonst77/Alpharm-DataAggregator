using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierRxOtc
{
    [Table("ClassifierRxOtc", Schema = "Classifier")]
    public class ClassifierRxOtc
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Classifierid { get; set; }
        public bool IsRx { get; set; }
        public Nullable<bool> IsChecked { get; set; }
        public Nullable<bool> IsException { get; set; }
    }
}
