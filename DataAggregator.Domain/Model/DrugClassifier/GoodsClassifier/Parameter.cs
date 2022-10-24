using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("Parameter", Schema = "GoodsClassifier")]
    public class Parameter
    {
        public long Id { get; set; }

        public long ParameterGroupId { get; set; }

        public long? ParentId { get; set; }

        public string Value { get; set; }

        [JsonIgnore]
        public virtual ParameterGroup ParameterGroup { get; set; }
    }
}