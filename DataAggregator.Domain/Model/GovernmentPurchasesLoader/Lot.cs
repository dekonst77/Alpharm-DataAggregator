using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Lot", Schema = "buffer")]
    public class Lot
    {
        public Lot()
        {
            this.PurchaseObject = new HashSet<PurchaseObject>();
        }

        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public int Number { get; set; }
        public decimal Sum { get; set; }
        public string SourceOfFinancing { get; set; }
        public long? DistributorId { get; set; }
        public Guid? LastChangedUserId { get; set; }
        public DateTime? LastChangedDate { get; set; }
        public Guid? LastChangedObjectsUserId { get; set; }
        public DateTime? LastChangedObjectsDate { get; set; }

        public virtual Purchase Purchase { get; set; }
        public virtual ICollection<PurchaseObject> PurchaseObject { get; set; } 
    }
}