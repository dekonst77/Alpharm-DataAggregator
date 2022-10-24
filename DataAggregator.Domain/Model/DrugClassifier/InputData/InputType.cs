using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.InputData
{
    [Table("InputType", Schema = "InputData")]
    public class InputType
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public virtual ICollection<InputDataSource> InputDataSource { get; set; }
    
    }
}