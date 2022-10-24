using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsProductionInfoParameter", Schema = "GoodsClassifier")]
    public class GoodsProductionInfoParameter
    {
        [Key, Column(Order = 0)]
        public long GoodsProductionInfoId { get; set; }
        [Key, Column(Order = 1)]
        public long ParameterId { get; set; }

        [JsonIgnore]
        public virtual Parameter Parameter { get; set; }
    }
}