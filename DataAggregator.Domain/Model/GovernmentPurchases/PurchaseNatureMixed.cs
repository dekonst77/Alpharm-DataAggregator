using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("PurchaseNatureMixed", Schema = "dbo")]
    public class PurchaseNatureMixed
    {
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public Byte NatureId { get; set; }
        public Int16? Nature_L2Id { get; set; }
        public decimal Percentage { get; set; }

        public virtual Purchase Purchase { get; set; }
        public virtual Nature Nature { get; set; }

        public virtual Nature_L2 Nature_L2 { get; set; }


    }
}