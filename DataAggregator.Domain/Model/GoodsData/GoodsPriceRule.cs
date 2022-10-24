using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GoodsData
{
    [Table("GoodsPriceRule", Schema = "calc")]
    public class GoodsPriceRule
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public long GoodsId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public DateTime? Date { get; set; }
    }
}
