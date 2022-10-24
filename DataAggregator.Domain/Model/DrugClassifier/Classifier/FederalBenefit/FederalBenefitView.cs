using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.FederalBenefit
{
    [Table("FederalBenefitView", Schema = "Classifier")]
    public class FederalBenefitView
    {
        [Key, Column(Order = 1)]
        public long? INNGroupId { get; set; }
        [Key, Column(Order = 2)]
        public long FormProductId { get; set; }
        [Key, Column(Order = 3)]
        public long FederalBenefitPeriodId { get; set; }
        
        public bool Checked { get; set; }

        public virtual FederalBenefitPeriod FederalBenefitPeriod { get; set; }

        public virtual INNGroup InnGroup { get; set; }

        public virtual FormProduct FormProduct { get; set; }

    }
}