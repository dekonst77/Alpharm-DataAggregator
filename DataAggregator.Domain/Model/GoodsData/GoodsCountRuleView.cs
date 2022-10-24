using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GoodsData
{
    [Table("GoodsCountRuleView", Schema = "calc")]
    public class GoodsCountRuleView
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public string RegionFullName { get; set; }

        #region

        public long? GoodsId { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public long? PackerId { get; set; }
        public string FullGoodsDescription { get; set; }
        public long? ClassifierId { get; set; }

        #endregion

        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string GoodsTradeName { get; set; }

        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public DateTime Date { get; set; }

        #region Distribution

        public long? DistributionGoodsId { get; set; }
        public long? DistributionOwnerTradeMarkId { get; set; }
        public long? DistributionPackerId { get; set; }
        public string DistributionFullGoodsDescription { get; set; }
        public long? DistributionClassifierId { get; set; }

        /// <summary>
        /// Торговое наименование - куда распределили
        /// </summary>
        public string DistributionGoodsTradeName { get; set; }        

        #endregion


        public int SellingSumPart { get; set; }
        public int? TopCountFrom { get; set; }
        public int? TopCountTo { get; set; }

        public bool RegionSpb { get; set; }
        public bool RegionMsk { get; set; }
        public bool RegionRus { get; set; }
    }
}