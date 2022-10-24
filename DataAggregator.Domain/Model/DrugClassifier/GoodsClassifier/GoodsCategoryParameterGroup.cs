using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsCategoryParameterGroup", Schema = "GoodsClassifier")]
    public class GoodsCategoryParameterGroup
    {
        [Key, Column(Order = 0)]
        public long GoodsCategoryId { get; set; }
        [Key, Column(Order = 1)]
        public long ParameterGroupId { get; set; }

        [JsonIgnore]
        public virtual ParameterGroup ParameterGroup { get; set; }
    }
}