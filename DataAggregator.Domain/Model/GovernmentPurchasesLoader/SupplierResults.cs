using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    
        [Table("SupplierResult", Schema = "buffer")]
        public class SupplierResult
        {
            public long Id { get; set; }
            public long LotStatusId { get; set; }
            public decimal? Sum { get; set; }
            public string PurchaseNumber { get; set; }
            public int MigrateStatusId { get; set; }
            public string ErrorMessage { get; set; }
            public string Winner { get; set; }
            public string ProtocolNumber { get; set; }
            public DateTime? ProtocolDate { get; set; }
            public long? SupplierId { get; set; }

            public virtual Supplier Supplier { get; set; }
            public virtual List<SupplierList> SupplierList { get; set; }
        }
   
}