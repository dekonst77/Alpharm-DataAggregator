using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("ContractInfo", Schema = "purchase")]
    public class ContractInfo
    {
        public long Id { get; set; }
        public string PurchaseNumber { get; set; }
        public string ReestrNumber { get; set; }
        public long? CommonPageId { get; set; }
        public long? ObjectsPageId { get; set; }
        public long? ProcessPageId { get; set; } 
        public long AnalyzeId { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusGeneratePurchaseId { get; set; }
        public DateTime Date { get; set; }
        public string url { get; set; }
        
    }
}