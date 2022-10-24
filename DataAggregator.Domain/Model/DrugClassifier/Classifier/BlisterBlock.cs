using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("BlisterBlock", Schema = "Classifier")]
    public class BlisterBlock
    {
        [Key]
        public long ClassifierId { get; set; }
        public Nullable<int> ClassifierPackingId { get; set; }
        public Nullable<bool> IsExist { get; set; }
        public string Comment { get; set; }
    }
}
