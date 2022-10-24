using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.InputData
{
    [Table("InputDataSource", Schema = "InputData")]
    public class InputDataSource
    {
        public long Id { get; set; }

        public string Value { get; set; }

        public long InputTypeId { get; set; }

        public long Version { get; set; }
      
        public InputType InputType { get; set; }

        public virtual ICollection<InputDrugDescription> InputDrugDescriptions { get; set; }
    }
}