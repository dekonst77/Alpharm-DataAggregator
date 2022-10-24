using System;
using System.Collections.Generic;
using DataAggregator.Domain.Model.GovernmentPurchases.View;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class SupplierResult
    {
        public long Id { get; set; }
        public long LotStatusId { get; set; }
        public long? SupplierRawId { get; set; }
        public string ProtocolNumber { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public long LotId { get; set; }
        public decimal? Sum { get; set; }
        public Guid? LastChangedUserId { get; set; }
        public DateTime? LastChangedDate { get; set; }

        public virtual Lot Lot { get; set; }

        public virtual List<SupplierList> SupplierList { get; set; }


        public virtual SupplierRaw SupplierRaw { get; set; }

        public virtual LotStatus LotStatus { get; set; }
        public bool ForCheck { get; set; }
    
        
    }
}
