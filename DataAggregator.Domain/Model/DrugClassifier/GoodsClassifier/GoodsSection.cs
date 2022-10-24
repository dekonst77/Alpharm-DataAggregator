using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsSection", Schema = "GoodsSystematization")]
    public class GoodsSection
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public virtual IList<GoodsCategory> GoodsCategory { get; set; }

    }
}