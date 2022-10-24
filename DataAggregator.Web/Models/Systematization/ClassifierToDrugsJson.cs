using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Systematization
{
    public class ClassifierToDrugsJson
    {
        public IList<long> DrugInWorkIdList { get; set; }

        public long? DrugId { get; set; }
        public long? GoodsId { get; set; }
        public bool IsOther { get; set; }
        public long PackerId { get; set; }

        public long OwnerTradeMarkId { get; set; }

        public int RealPackingCount { get; set; }
    }
}