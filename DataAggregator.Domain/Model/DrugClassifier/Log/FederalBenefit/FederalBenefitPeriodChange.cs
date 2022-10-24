using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Log.FederalBenefit
{
    [Table("FederalBenefitPeriodChange", Schema = "Log")]
    public class FederalBenefitPeriodChange
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long FederalBenefitPeriodId { get; set; }
        public int? YearStart { get; set; }
        public int? MonthStart { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }
        public Guid UserId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
        public long ActionTypeId { get; set; }

        public virtual ActionType ActionType { get; set; }
    }
}
