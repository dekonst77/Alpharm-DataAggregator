using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.View
{
    [Table("CountRuleFullVolumeView", Schema = "calc")]
    public class CountRuleFullVolumeView
    {
        public long Id { get; set; }

        public Guid ChangeUserId { get; set; }
        public string ChangeSurname { get; set; }
        public DateTime ChangeDate { get; set; }

        public int? YearStart { get; set; }
        public int? MonthStart { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }


        #region Источник

        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public string FullDrugDescription { get; set; }
        public long ClassifierId { get; set; }

        #endregion

        #region Назначение

        public long? DistributionDrugId { get; set; }
        public long? DistributionOwnerTradeMarkId { get; set; }
        public long? DistributionPackerId { get; set; }
        public string DistributionFullDrugDescription { get; set; }
        public long? DistributionClassifierId { get; set; }

        #endregion

        public int? TopCountTo { get; set; }
    }
}