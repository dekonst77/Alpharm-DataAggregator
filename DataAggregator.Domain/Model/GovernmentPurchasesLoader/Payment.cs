using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Payment", Schema = "buffer")]
    public class Payment
    {
        public long Id { get; set; }

        public long PurchaseId { get; set; }

        public string KBK { get; set; }

        public string Year { get; set; }

        public decimal Sum { get; set; }

        public virtual Purchase Purchase { get; set; }
   
    }
}