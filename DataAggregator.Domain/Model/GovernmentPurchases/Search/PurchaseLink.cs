using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Search
{
    [Table("PurchaseLink", Schema = "Search")]
    public class PurchaseLink
    {
        public long Id { get; set; }

        public string PurchaseNumber { get; set; }

        public Byte LawTypeId { get; set; }

        public string PurchaseUrl { get; set; }

        public DateTime AddDate { get; set; }

        public Guid UserGuid { get; set; }
    }
}
