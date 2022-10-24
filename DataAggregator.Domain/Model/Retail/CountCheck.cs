using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class CountCheck
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public decimal Coefficient { get; set; }
        public decimal SellingCount { get; set; }
        public decimal AvgSellingCountHalfYear { get; set; }
    }
}
