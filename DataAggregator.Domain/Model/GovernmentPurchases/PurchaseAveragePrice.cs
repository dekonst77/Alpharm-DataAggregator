using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("PurchaseAveragePrice", Schema = "calc")]
    public class PurchaseAveragePrice
    {
        public long Id { get; set; }
        public long RegionId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long ClassifierId { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime LastChangedDate { get; set; }
        public Guid? LastChangedUserId { get; set; }
    }

    [Table("AutoCorrectAmountInfo", Schema = "dbo")]
    public class AutoCorrectAmountInfo
    {
        [Key]
        public string Unit { get; set; }
        public int Type { get; set; }
    }
    [Table("UnitType", Schema = "dbo")]
    public class UnitType
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }

    [Table("AutoCorrectDosageRecount", Schema = "dbo")]
    public class AutoCorrectDosageRecount
    {
        [Key]
        public long Id { get; set; }
        public long? DosageGroupId { get; set; }
        public long FormProductId { get; set; }
        public int ConsumerPackingCount { get; set; }

        public decimal? CoeffConsumerPackingCount { get; set; }
        public decimal? CoeffMgAmount { get; set; }
        public decimal? CoeffMlAmount { get; set; }
        public decimal? CoeffGAmount { get; set; }
        public decimal? CoeffDosAmount { get; set; }
        public decimal? CoeffMeAmount { get; set; }

        public decimal? CoeffKgAmount { get; set; }
        public decimal? CoeffLAmount { get; set; }
        public decimal? CoeffM3Amount { get; set; }
    }
}
