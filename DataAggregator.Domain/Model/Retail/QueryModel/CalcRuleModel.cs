using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;

namespace DataAggregator.Domain.Model.Retail.QueryModel
{
    public class CalcRuleModel
    {
        [Key, Column(Order = 1)]
        public int ClassifierId { get; set; }

        public long BrandId { get; set; }
        public string Brand { get; set; }
        public string DrugDescription { get; set; }

        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string TradeName { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        [DecimalPrecision(38, 6)]
        public decimal? SellingSumNDS { get; set; }
        public decimal? PurchaseSumNDS { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? PurchaseCount { get; set; }
        public decimal? SellingSumNDSPart { get; set; }
        public decimal? PurchaseSumNDSPart { get; set; }
        public decimal? SellingCountPart { get; set; }
        public decimal? PurchaseCountPart { get; set; }

        public bool IsInCountRules { get; set; }

    }


    public class CalcRuleModel2
    {
        [Key, Column(Order = 1)]
        public int ClassifierId { get; set; }

        public int BrandId { get; set; }
        public string Brand { get; set; }
        public string DrugDescription { get; set; }
        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string TradeName { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public string SellingSumNDS { get; set; }
        public string PurchaseSumNDS { get; set; }
        public string SellingCount { get; set; }
        public string PurchaseCount { get; set; }
        public string SellingSumNDSPart { get; set; }
        public string PurchaseSumNDSPart { get; set; }
        public string SellingCountPart { get; set; }
        public string PurchaseCountPart { get; set; }

        public bool IsInCountRules { get; set; }

    }
}