using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("LoadTask", Schema = "Purchase")]
    public class LoadTask
    {
        public long Id { get; set; }
        public long Type { get; set; }
        public string Url { get; set; }
        public string PurchaseNumber { get; set; }
        public bool IsLoaded { get; set; }
        public DateTime DateAdd { get; set; }
        public string ReestrNumber { get; set; }
        public DateTime? LastTryLoad { get; set; }
        public int? TryCount { get; set; }
        public string ErrorMessage { get; set; }
    }
}