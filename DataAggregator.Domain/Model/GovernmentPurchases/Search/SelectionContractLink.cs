using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Search
{
    [Table("SelectionContractLink", Schema = "Search")]
    public class SelectionContractLink
    {
        public long Id { get; set; }

        public string PurchaseNumber { get; set; }

        public string ReestrNumber { get; set; }
    }
}
