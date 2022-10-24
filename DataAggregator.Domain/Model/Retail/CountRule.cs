using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("CountRule", Schema = "calc")]
    public class CountRule
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }
        public string RegionCode { get; set; }
        public int? ClassifierId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
#warning переименовать поля в БД
        public int? DistributionClassifierId { get; set; }
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