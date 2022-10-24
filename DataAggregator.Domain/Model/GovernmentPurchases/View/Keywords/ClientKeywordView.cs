using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Keywords
{
    [Table("ClientKeywordView", Schema = "Keywords")]
    public class ClientKeywordView
    {
        public long Id { get; set; }

        public long ClientId { get; set; }

        public long KeywordId { get; set; }

        public string KeywordText { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime DateStart { get; set; }
        
        public string StartUser { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime? DateEnd { get; set; }
        
        public string EndUser { get; set; }
    }
}
