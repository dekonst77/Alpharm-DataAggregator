namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class SupplierList
    {
        public long Id { get; set; }
        public long SupplierResultId { get; set; }
        public long? SupplierRawId { get; set; }
        public decimal? Sum { get; set; }
        public int? Number { get; set; }
        public bool IsWinner { get; set; }
        public long? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual SupplierRaw SupplierRaw { get; set; }

        public virtual SupplierResult SupplierResult { get; set; }
     
    }
}
