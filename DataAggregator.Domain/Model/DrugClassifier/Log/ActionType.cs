using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Log
{

    [Table("ActionType", Schema = "Log")]
    public class ActionType
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
}
