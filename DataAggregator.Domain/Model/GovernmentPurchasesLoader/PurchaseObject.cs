using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("PurchaseObject", Schema = "buffer")]
    public class PurchaseObject
    {
        public long Id { get; set; }
        public long LotId { get; set; }
        public string Name { get; set; }
        public string OKPD { get; set; }
        public string Unit { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }
        public bool NewOKPD { get; set; }

        public virtual Lot Lot { get; set; } 
    }
}