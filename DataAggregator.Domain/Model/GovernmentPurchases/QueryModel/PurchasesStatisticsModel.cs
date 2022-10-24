using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.QueryModel
{
    public class PurchasesStatisticsModel
    {
        public string ClassName { get; set; }
        public string StatusName { get; set; }
        public int Count { get; set; }
        public Byte KK { get; set; }
    }
}