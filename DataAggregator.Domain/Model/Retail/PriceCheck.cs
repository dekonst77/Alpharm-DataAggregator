using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class PriceCheck
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public decimal AvgSellingPrice { get; set; }
        public decimal AvgPurchasePrice { get; set; }
        public decimal AvgSellingCount { get; set; }
        public decimal AvgPurchaseCount { get; set; }
        public decimal SellingSum { get; set; }
        public decimal PurchaseSum { get; set; }
        public decimal PriceDeviation { get; set; }
    }
}
