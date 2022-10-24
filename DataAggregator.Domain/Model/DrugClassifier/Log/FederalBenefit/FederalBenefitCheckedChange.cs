using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Log.FederalBenefit
{


    [Table("FederalBenefitCheckedChange", Schema = "Log")]
    public class FederalBenefitCheckedChange
    {
        public long Id { get; set; }
        public long INNGroupId { get; set; }
        public long FormProductId { get; set; }
        public Guid UserId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
        public long ActionTypeId { get; set; }

        public virtual ActionType ActionType { get; set; }
    }
}