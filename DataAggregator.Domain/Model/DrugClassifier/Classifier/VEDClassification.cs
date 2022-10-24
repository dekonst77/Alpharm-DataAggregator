using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("VEDClassification", Schema = "Classifier")]
    public class VEDClassification
    {
        [Key, Column(Order = 1)]
        public long? INNGroupId { get; set; }
        [Key, Column(Order = 2)]
        public long FormProductId { get; set; }
        [Key, Column(Order = 3)]
        public long VEDPeriodId { get; set; }

        public virtual VEDPeriod VEDPeriod { get; set; }

        public virtual INNGroup InnGroup { get; set; }

        public virtual FormProduct FormProduct { get; set; }

    }
}