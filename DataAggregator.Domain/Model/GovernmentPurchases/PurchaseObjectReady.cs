using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("PurchaseObjectReady", Schema = "dbo")]
    public class PurchaseObjectReady
    {
        public long Id { get; set; }

        public long LotId { get; set; }

        public string Name { get; set; }

        public string OKPD { get; set; }

        public string Unit { get; set; }

        public decimal Amount { get; set; }

        public decimal? Price { get; set; }

        public decimal? Sum { get; set; }

        public long? ReceiverId { get; set; }
        public string ReceiverRaw { get; set; }

        public long? DrugRawId { get; set; }

        public long? DrugClassifierId { get; set; }

        public decimal? AmountCorrected { get; set; }

        public decimal? PriceCorrected { get; set; }

        public decimal? SumCorrected { get; set; }

        public long? ClassifierId { get; set; }

        public virtual Lot Lot { get; set; }

        public bool VNC { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual OrganizationOut ReceiverOut { get; set; }

        public virtual PurchaseObjectCalculated PurchaseObjectCalculated { get; set; }
    }
}