using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GoodsData
{
    [Table("GoodsPriceRuleListView", Schema = "calc")]
    public class GoodsPriceRuleListView
    {
        [Key]
        [Column(Order = 1)]
        public long PriceRuleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 3)]
        public int Month { get; set; }

        /// <summary>
        /// Дата изменения правила
        /// </summary>
        public DateTime? Date { get; set; }
        public string RegionCode { get; set; }
        public string RegionFullName { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public long GoodsId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public string GoodsTradeName { get; set; }
        public string Brand { get; set; }
        public string GoodsDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public string Comment { get; set; }
    }
}
