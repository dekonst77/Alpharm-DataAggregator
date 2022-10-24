using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("VEDChecked", Schema = "Classifier")]
    public class VEDChecked
    {
        [Key, Column(Order = 1)]
        public long INNGroupId { get; set; }
        [Key, Column(Order = 2)]
        public long FormProductId { get; set; }
    }
}