using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GoodsData
{
    [Table("GoodsCountRule", Schema = "calc")]
    public class GoodsCountRule
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public long? GoodsId { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public long? PackerId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public long? DistributionGoodsId { get; set; }
        public long? DistributionOwnerTradeMarkId { get; set; }
        public long? DistributionPackerId { get; set; }
        public int? SellingSumPart { get; set; }
        public int? TopCountFrom { get; set; }
        public int? TopCountTo { get; set; }
        public bool RegionSpb { get; set; }
        public bool RegionMsk { get; set; }
        public bool RegionRus { get; set; }
    }
}