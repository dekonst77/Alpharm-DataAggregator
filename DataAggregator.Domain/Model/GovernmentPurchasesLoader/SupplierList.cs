using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("SupplierList", Schema = "buffer")]
    public class SupplierList
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string ClearName { get; set; }

        public decimal? Sum { get; set; }

        public int? Number { get; set; }

        public bool IsWinner { get; set; }

        public long SupplierResultId { get; set; }

        public long SupplierId { get; set; }

        public virtual SupplierResult SupplierResult { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}