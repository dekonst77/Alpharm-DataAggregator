using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{

    public class Lot
    {
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

        public Guid? LastChangedObjectsCorrectedUserId { get; set; }

        public DateTime? LastChangedObjectsCorrectedDate { get; set; }

        [JsonIgnore]
        public virtual Purchase Purchase { get; set; }

        [JsonIgnore]
        public virtual IList<PurchaseObject> Object { get; set; }

        [JsonIgnore]
        public virtual IList<PurchaseObjectReady> PurchaseObjectReady { get; set; }

        public virtual IList<LotFunding> LotFunding { get; set; }

        [JsonIgnore]
        public virtual IList<SupplierResult> SupplierResult { get; set; }
    }
}
