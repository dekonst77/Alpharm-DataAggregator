using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("PriceRule", Schema = "calc")]
    public class PriceRule
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public decimal? PurchasePriceMin { get; set; }
        public decimal? PurchasePriceMax { get; set; }
        public int ClassifierId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public DateTime? Date { get; set; }
    }
}
