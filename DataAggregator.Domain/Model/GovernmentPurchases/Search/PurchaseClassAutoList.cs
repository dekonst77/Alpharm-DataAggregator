using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Search
{
    [Table("PurchaseClassAutoList", Schema = "Search")]
    public class PurchaseClassAutoList
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public long ListTypeId { get; set; }
        public Guid StartUserId{ get; set; }
        public Guid? EndUserId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool? Recheck { get; set; }

        public virtual ListType ListType { get; set; }
    }
}
