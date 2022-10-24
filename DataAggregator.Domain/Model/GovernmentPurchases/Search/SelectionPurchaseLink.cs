using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Search
{
    [Table("SelectionPurchaseLink", Schema = "Search")]
    public class SelectionPurchaseLink
    {
        public long Id { get; set; }

        public string Number { get; set; }
    }
}
