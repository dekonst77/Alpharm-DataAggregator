using System;

namespace DataAggregator.Web.Models.GovernmentPurchases.DistributionKeyWords
{
    public class PurchaseClassAutoListJson
    {
        public long? Id { get; set; }
        public string Value { get; set; }
        public string ListTypeValue { get; set; }
        public string StartUser { get; set; }
        public string EndUser { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool Recheck { get; set; }
    }
}