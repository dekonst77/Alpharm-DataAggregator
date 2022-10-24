using System;
using System.ComponentModel.DataAnnotations;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class NotExportedToExternalContractsView
    {
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        [Key]
        public long ContractId { get; set; }
        public string ContrReestrNumber { get; set; }
        public decimal? ContractSum { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime PurchaseDateBegin { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime PurchaseDateEnd { get; set; }
        public int BadCount { get; set; }
        public int BadObjects { get; set; }
        public int BadNature { get; set; }
        public int BadDeliveryTimeInfo { get; set; }
        public int BadLotFunding { get; set; }
        public int BadObjSum { get; set; }
    }
}
