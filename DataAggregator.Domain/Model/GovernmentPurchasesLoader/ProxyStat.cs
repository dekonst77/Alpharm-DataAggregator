using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("ProxyStat", Schema = "dbo")]
    public class ProxyStat
    {
        [Column(Order = 0), Key]
        public long ProxyId { get; set; }
        [Column(Order = 1), Key]
        public int Year { get; set; }
        [Column(Order = 2), Key]
        public int Month { get; set; }
        [Column(Order = 3), Key]
        public int Day { get; set; }
        public int Page { get; set; }
    }
}