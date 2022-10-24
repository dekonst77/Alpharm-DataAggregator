using System.Collections.Generic;

namespace DataAggregator.Web.Models
{
    public class ClassifierToGoodsJson
    {
        public IList<long> GoodsInWorkIdList { get; set; }

        public long GoodsId { get; set; }

        public long PackerId { get; set; }

        public long OwnerTradeMarkId { get; set; }
    }
}