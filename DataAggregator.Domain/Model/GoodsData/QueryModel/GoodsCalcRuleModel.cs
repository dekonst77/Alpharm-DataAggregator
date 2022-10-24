using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GoodsData.QueryModel
{
    public class GoodsCalcRuleModel
    {
        [Key, Column(Order = 1)]
        public long GoodsId { get; set; }
        [Key, Column(Order = 2)]
        public long OwnerTradeMarkId { get; set; }
        [Key, Column(Order = 3)]
        public long PackerId { get; set; }
        public long? BrandId { get; set; }
        public string Brand { get; set; }
        public string GoodsDescription { get; set; }

        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string GoodsTradeName { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public decimal? SellingSumNDS { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? SellingSumNDSPart { get; set; }
        public decimal? SellingCountPart { get; set; }

        public bool IsInCountRules { get; set; }

        public long ClassifierId { get; set; }
    }
}