using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.View
{
    [Table("CountRuleView", Schema = "calc")]
    public class CountRuleView
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }
        public string RegionCode { get; set; }
        public string RegionFullName { get; set; }
        #region

        public string FullDrugDescription { get; set; }
        public long? ClassifierId { get; set; }
        #endregion
        /// <summary>
        /// Торговое наименование
        /// </summary>
        public string TradeName { get; set; }
        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public DateTime Date { get; set; }
        #region Distribution
#warning переименовать поля в БД

        [Column("DistrFullDrugDescription")]
        public string DistributionFullDrugDescription { get; set; }
        [Column("DistrClassifierId")]
        public long? DistributionClassifierId { get; set; }

        /// <summary>
        /// Торговое наименование - куда распределили
        /// </summary>
        [Column("DistrTradeName")]
        public string DistributionTradeName { get; set; }

        #endregion


        public int? PurchaseSumPart { get; set; }
        public int? SellingSumPart { get; set; }
        public int? TopCountFrom { get; set; }
        public int? TopCountTo { get; set; }

        public bool RegionSpb { get; set; }
        public bool RegionMsk { get; set; }
        public bool RegionRus { get; set; }

        public decimal? PurchaseCount { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? PurchaseSum { get; set; }
        public decimal? SellingSum { get; set; }
       




    }
}