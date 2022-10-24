using System;
using DataAggregator.Domain.Model.GoodsData;

namespace DataAggregator.Web.Models.Retail.GoodsCountRuleFullVolumeEditor
{
    public class GoodsCountRuleFullVolumeModel
    {
        public long? Id { get; set; }

        public string ChangeSurname { get; set; }
        public DateTime ChangeDate { get; set; }

        public int? YearStart { get; set; }
        public int? MonthStart { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }

        #region Источник

        public long? GoodsId { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public long? PackerId { get; set; }
        public string FullGoodsDescription { get; set; }
        public long? ClassifierId { get; set; }

        #endregion

        #region Назначение

        public long? DistributionGoodsId { get; set; }
        public long? DistributionOwnerTradeMarkId { get; set; }
        public long? DistributionPackerId { get; set; }
        public string DistributionFullGoodsDescription { get; set; }
        public long? DistributionClassifierId { get; set; }

        #endregion

        public int? TopCountTo { get; set; }

        public static GoodsCountRuleFullVolumeModel Create(GoodsCountRuleFullVolumeView view)
        {
            return ModelMapper.Mapper.Map<GoodsCountRuleFullVolumeModel>(view);
        }
    }
}