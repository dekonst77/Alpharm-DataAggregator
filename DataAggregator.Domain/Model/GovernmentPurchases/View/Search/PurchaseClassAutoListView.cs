using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Search
{
    [Table("PurchaseClassAutoListView", Schema = "Search")]
    public class PurchaseClassAutoListView
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string ListTypeValue { get; set; }
        public string StartUser { get; set; }
        public string EndUser { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime DateStart { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime? DateEnd { get; set; }
        public bool Recheck { get; set; }
    }
}
