using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.FederalBenefit
{
    [Table("FederalBenefitPeriod", Schema = "Classifier")]
    public class FederalBenefitPeriod
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int YearStart { get; set; }
        public int MonthStart { get; set; }
        public int YearEnd { get; set; }
        public int MonthEnd { get; set; }
    }
}