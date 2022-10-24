using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class SQAView
    {
        [Key]
        public int Id { get; set; }
        public long? INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public long? DosageGroupId { get; set; }
        public string DosageGroup { get; set; }

        public long? FormProductId { get; set; }
        public string FormProduct { get; set; }

        public bool IsSQA { get; set; }
    }
}
