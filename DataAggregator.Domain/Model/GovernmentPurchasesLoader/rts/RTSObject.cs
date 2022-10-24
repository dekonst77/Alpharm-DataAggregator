using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts
{
    [Table("Object", Schema = "rts")]
    public class RTSObject
    {
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public string Name { get; set; }
        public string OKPD { get; set; }
        public string Unit { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sum { get; set; }

        public virtual RTSPurchase Purchase { get; set; } 
    }
}
