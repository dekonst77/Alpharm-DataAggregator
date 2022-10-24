using DataAggregator.Domain.Model.Retail;
using DataAggregator.Domain.Model.Retail.View;
using System;

namespace DataAggregator.Web.Models.Retail.CountRuleEditor
{
    public class CountRuleModel
    {
        public long? Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }
        public string RegionCode { get; set; }
        public string Region { get; set; }

        public string FullDrugDescription { get; set; }
        public int? ClassifierId { get; set; }
        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string TradeName { get; set; }
        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public DateTime Date { get; set; }
        public string DistributionFullDrugDescription { get; set; }
        public int? DistributionClassifierId { get; set; }
        /// <summary>
        /// Торговое наименование - куда распределили
        /// </summary>
        public string DistributionTradeName { get; set; }
        public int? SellingSumPart { get; set; }
        public int? PurchaseSumPart { get; set; }
        public int? TopCountFrom { get; set; }
        public int? TopCountTo { get; set; }
        public bool RegionMsk { get; set; }
        public bool RegionSpb { get; set; }
        public bool RegionRus { get; set; }
        public bool InUsed { get; set; }
        public bool OutUsed { get; set; }
        public decimal? PurchaseCount { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? PurchaseSum { get; set; }
        public decimal? SellingSum { get; set; }
        public string Flag { get; set; }


        public CountRuleModel()
        {
        }

        public static CountRuleModel Create(CountRuleView model)
        {
            return ModelMapper.Mapper.Map<CountRuleModel>(model);
        }

        /// <summary>
        /// Заполним model 
        /// </summary>
        public void SetCountRule(CountRule model, Guid userId)
        {

            model.Year = Year;
            model.ClassifierId = ClassifierId;
            model.DistributionClassifierId = DistributionClassifierId;
            model.Month = Month;
            model.YearEnd = YearEnd;
            model.MonthEnd = MonthEnd;
            model.RegionCode = RegionCode;
            model.Date = DateTime.Now;
            model.UserId = userId;
            model.SellingSumPart = SellingSumPart;
            model.PurchaseSumPart = PurchaseSumPart;
            model.TopCountFrom = TopCountFrom == 0 ? null : TopCountFrom;
            model.TopCountTo = TopCountTo == 0 ? null : TopCountTo;
            //model.RegionMsk = RegionMsk;
            //model.RegionSpb = RegionSpb;
            //model.RegionRus = RegionRus;
            model.SellingCount = SellingCount;
            model.PurchaseCount = PurchaseCount;
            model.SellingSum = SellingSum;
            model.PurchaseSum = PurchaseSum;

        }
    }
}