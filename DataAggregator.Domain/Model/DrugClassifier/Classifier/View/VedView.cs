using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    [Table("VEDView", Schema = "Classifier")]
    public class VedView
    {
        [Key, Column(Order = 1)]
        public long? INNGroupId { get; set; }
        [Key, Column(Order = 2)]
        public long FormProductId { get; set; }
        [Key, Column(Order = 3)]
        public long VEDPeriodId { get; set; }
        
        public bool Checked { get; set; }

        public virtual VEDPeriod VEDPeriod { get; set; }

        public virtual INNGroup InnGroup { get; set; }

        public virtual FormProduct FormProduct { get; set; }

    }
}