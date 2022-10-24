using System.Collections.Generic;
using DataAggregator.Core.Filter;

namespace DataAggregator.Web.Models.GoodsSystematization
{
    public class GoodsFilterResultJson
    {
        public int Count { get; set; }

        public List<long?> ForWorkCategoryIds { get; set; }

        public List<long?> ForAddingCategoryIds { get; set; }

        public List<string> UserGuids { get; set; }
        public AdditionalGoodsFilter Additional { get; set; }

        public GoodsFilterResultJson()
        {
            ForWorkCategoryIds = new List<long?>();
            ForAddingCategoryIds = new List<long?>();
            UserGuids = new List<string>();
        }
    }
}