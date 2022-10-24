using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier
{
    [Table("GoodsCategory", Schema = "GoodsSystematization")]
    public class GoodsCategory
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long GoodsSectionId { get; set; }

        [JsonIgnore]
        public virtual IList<GoodsCategoryKeyword> GoodsCategoryKeywords { get; set; }

        [JsonIgnore]
        public virtual IList<GoodsClear> GoodsClear { get; set; }

        public virtual GoodsSection GoodsSection { get; set; }
    }
}