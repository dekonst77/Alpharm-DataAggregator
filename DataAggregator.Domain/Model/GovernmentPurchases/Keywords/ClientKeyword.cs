using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Keywords
{
    [Table("ClientKeyword", Schema = "Keywords")]
    public class ClientKeyword
    {
        public long Id { get; set; }

        public long ClientId { get; set; }

        public long KeywordId { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime DateStart { get; set; }
        
        public Guid StartUserId { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime? DateEnd { get; set; }
        
        public Guid? EndUserId { get; set; }

        public virtual Client Client { get; set; }
        
        public virtual Keyword Keyword { get; set; }
    }
}
