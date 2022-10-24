using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class SQA
    {
        [Key]
        public int Id { get; set; }
        public long? INNGroupId { get; set; }
        public long? DosageGroupId { get; set; }
        public long? FormProductId { get; set; }
        public bool IsSQA { get; set; }

    }
}
