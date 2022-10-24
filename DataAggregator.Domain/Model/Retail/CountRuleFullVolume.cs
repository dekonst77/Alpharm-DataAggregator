using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("CountRuleFullVolume", Schema = "calc")]
    public class CountRuleFullVolume
    {
        public long Id { get; set; }

        public Guid ChangeUserId { get; set; }
        public DateTime ChangeDate { get; set; }

        public int? YearStart { get; set; }
        public int? MonthStart { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }


        #region Источник

        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }

        #endregion

        #region Назначение

        public long? DistributionDrugId { get; set; }
        public long? DistributionOwnerTradeMarkId { get; set; }
        public long? DistributionPackerId { get; set; }        
        
        #endregion

        public int? TopCountTo { get; set; }
    }
}