using System.Collections.Generic;
using DataAggregator.Core.Filter;
using DataAggregator.Domain.Model.DrugClassifier.Stat;

namespace DataAggregator.Web.Models.GoodsSystematization
{
    public class GoodsFilterJson
    {
        public int Count { get; set; }

        public List<GoodsCategoryStatJson> CategoryStat { get; set; }

        public List<GoodsUserStat> UserStat { get; set; }

        public AdditionalFilter Additional { get; set; }

        public GoodsFilterJson()
        {
            CategoryStat = new List<GoodsCategoryStatJson>();
            UserStat = new List<GoodsUserStat>();
        }
    }
}