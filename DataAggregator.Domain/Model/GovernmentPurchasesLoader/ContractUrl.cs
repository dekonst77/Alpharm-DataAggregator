using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("ContractUrl", Schema = "purchase")]
    public class ContractUrl
    {
        public long Id { get; set; }
        public string ReestrNumber {get;set;}
        public string PurchaseNumber { get; set; }
    }
}