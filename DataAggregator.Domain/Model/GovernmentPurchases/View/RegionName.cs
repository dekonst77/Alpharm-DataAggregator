using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class RegionName
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
        public int? Level { get; set; }
    }
}
