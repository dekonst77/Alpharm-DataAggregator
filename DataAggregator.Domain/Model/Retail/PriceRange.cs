using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class PriceRange
    {
        public long Id { get; set; }
        public string RegionCode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public decimal? PurchasePriceMin { get; set; }
        public decimal? PurchasePriceMax { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public bool IsActual { get; set; }
    }
}
