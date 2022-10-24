using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Search
{
    [Table("ContractLink", Schema = "Search")]
    public class ContractLink
    {
        public long Id { get; set; }

        public string PurchaseNumber { get; set; }

        public Byte LawTypeId { get; set; }

        public string ReestrNumber { get; set; }

        public DateTime AddDate { get; set; }

        public Guid UserGuid { get; set; }
    }
}
