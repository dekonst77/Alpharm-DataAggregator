using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.FederalBenefit
{
    [Table("FederalBenefitChecked", Schema = "Classifier")]
    public class FederalBenefitChecked
    {
        [Key, Column(Order = 1)]
        public long INNGroupId { get; set; }
        [Key, Column(Order = 2)]
        public long FormProductId { get; set; }
    }
}