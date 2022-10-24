using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class PriceCheckView
    {
        [Key]
        [Column(Order = 1)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Month { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RegionCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public long DrugId { get; set; }
        [Key]
        [Column(Order = 5)]
        public long OwnerTradeMarkId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public decimal AvgSellingPrice { get; set; }
        public decimal AvgPurchasePrice { get; set; }
        public decimal AvgSellingCount { get; set; }
        public decimal AvgPurchaseCount { get; set; }
        public decimal SellingSum { get; set; }
        public decimal PurchaseSum { get; set; }
        public decimal PriceDeviation { get; set; }
        public decimal? PurchasePriceMin { get; set; }
        public decimal? PurchasePriceMax { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public bool? IsActual { get; set; }
        public long? PriceRangeId { get; set; }
        public string RegionName { get; set; }
    }
}
