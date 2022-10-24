using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("VEDClassificationByTN", Schema = "Classifier")]
    public class VEDClassificationByTN
    {
        [Key, Column(Order = 1)]
        public long? TradeNameId { get; set; }
        [Key, Column(Order = 2)]
        public long FormProductId { get; set; }
        [Key, Column(Order = 3)]
        public long VEDPeriodId { get; set; }

        public virtual TradeName TradeName { get; set; }

        public virtual FormProduct FormProduct { get; set; }

        public virtual VEDPeriod VEDPeriod { get; set; }

    }
}