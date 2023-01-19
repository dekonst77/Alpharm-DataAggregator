using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Contract
    {
        public long Id { get; set; }

        public string ReestrNumber { get; set; }

        public string Url { get; set; }

        public long? ContractStatusId { get; set; }

        public long? ReceiverId { get; set; }

        public DateTime? ConclusionDate { get; set; }

        public DateTime? StatusDate { get; set; } 

        public string ContractNumber { get; set; }

        public long? MethodPriceId { get; set; }

        public decimal? Sum { get; set; }

        public long? CurrencyId { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public long? SupplierRawId { get; set; }

        public decimal? ActuallyPaid { get; set; }

        public long LotId { get; set; }

        public DateTime? DateCreate { get; set; }

        public DateTime? DateUpdate { get; set; }

        public Guid? LastChangedUserId { get; set; }

        public DateTime? LastChangedDate { get; set; }

        public Guid? LastChangedObjectsUserId { get; set; }

        public DateTime? LastChangedObjectsDate { get; set; }

        public Byte KK { get; set; }

        public virtual Lot Lot { get; set; }

        public long? SupplierId { get; set; }

        public Guid? LastChangedObjectsCorrectedUserId { get; set; }

        public DateTime? LastChangedObjectsCorrectedDate { get; set; }
        public string change_reason { get; set; }
        public string change_objects { get; set; }

        public virtual ContractStatus ContractStatus  { get; set; }

        public virtual Organization Receiver { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual SupplierRaw SupplierRaw { get; set; }

    }

    public class ContractKK
    {
        public Byte Id { get; set; }
        public string Name { get; set; }
    }
}
