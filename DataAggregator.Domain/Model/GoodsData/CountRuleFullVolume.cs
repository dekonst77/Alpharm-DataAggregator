using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GoodsData
{
    [Table("GoodsCountRuleFullVolume", Schema = "calc")]
    public class GoodsCountRuleFullVolume
    {
        public long Id { get; set; }

        public Guid ChangeUserId { get; set; }
        public DateTime ChangeDate { get; set; }

        public int? YearStart { get; set; }
        public int? MonthStart { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }


        #region Источник

        public long GoodsId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }

        #endregion

        #region Назначение

        public long? DistributionGoodsId { get; set; }
        public long? DistributionOwnerTradeMarkId { get; set; }
        public long? DistributionPackerId { get; set; }        
        
        #endregion

        public int? TopCountTo { get; set; }
    }
}