using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("ParameterGroup", Schema = "GoodsClassifier")]
    public class ParameterGroup
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long GoodsCategoryId { get; set; }

        [JsonIgnore]
        public virtual IList<Parameter> Parameter { get; set; }

        [JsonIgnore]
        public virtual GoodsCategory GoodsCategory { get; set; }
    }
}