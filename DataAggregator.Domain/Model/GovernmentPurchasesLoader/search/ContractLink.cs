using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.search
{
    [Table("ContractLink", Schema = "search")]
    public class ContractLink
    {
        public long Id { get; set; }
        public string PurchaseNumber { get; set; }
        public string ReestrNumber { get; set; }
        public long? ContractSearchTaskId { get; set; }
        public string PurchaseUrl { get; set; }
    }
}
