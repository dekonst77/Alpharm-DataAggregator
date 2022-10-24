using System.Collections.Generic;

namespace DataAggregator.Web.Models
{
    public class GoodsCategoryJson
    {
        public IList<long> GoodsInWorkIdList { get; set; }

        public long GoodsCategoryId { get; set; }
    }
}