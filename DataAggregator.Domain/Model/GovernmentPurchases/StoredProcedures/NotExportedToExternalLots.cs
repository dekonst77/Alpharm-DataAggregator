using System;
using System.ComponentModel.DataAnnotations;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.StoredProcedures
{
    public class NotExportedToExternalLots
    {
        [Key]
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public string PurchaseName { get; set; }
        public long LotId { get; set; }
        public int LotNumber { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime PurchaseDateBegin { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime PurchaseDateEnd { get; set; }
        public decimal? LotSum { get; set; }
        public int BadCount { get; set; }
        public int BadLotSum { get; set; }
        public int BadObjects { get; set; }
        public int BadNature { get; set; }
        public int BadDeliveryTimeInfo { get; set; }
        public int BadLotFunding { get; set; }
        public int BadCoefficient { get; set; }
        public int BadObjSum { get; set; }
    }
}
