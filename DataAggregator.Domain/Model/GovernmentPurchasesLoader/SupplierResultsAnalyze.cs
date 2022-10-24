using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("SupplierResult", Schema = "purchase")]
    public class SupplierResultAnalyze
    {
        public long Id { get; set; }
        public string PurchaseNumber { get; set; }
        public long PageId { get; set; }
        public DateTime DateLoad { get; set; }
        public int AnalyzeId { get; set; }
        public string ErrorMessage { get; set; }
    }

   
}