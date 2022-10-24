using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View.Search
{
    [Table("ContractLinkView", Schema = "Search")]
    public class ContractLinkView
    {
        public long Id { get; set; }

        public string PurchaseNumber { get; set; }

        public string LawTypeName { get; set; }

        public string ReestrNumber { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime? AddDate { get; set; }

        public string UserName { get; set; }
        public string StatusEnd { get; set; }
    }
}
