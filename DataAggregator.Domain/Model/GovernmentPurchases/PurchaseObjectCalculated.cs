using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("PurchaseObjectCalculated", Schema = "calc")]
    public class PurchaseObjectCalculated
    {
        public long Id { get; set; }

        public long PurchaseObjectReadyId { get; set; }

        public decimal Amount { get; set; }

        public decimal? Price { get; set; }

        public decimal? Sum { get; set; }

        public long? ReceiverId { get; set; }

        public long? DrugId { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public long? PackerId { get; set; }

        public Byte? RecoveryTypeId { get; set; }

        public bool? IsOther { get; set; }
        public Int16 type { get; set; }

        public decimal? Coefficient { get; set; }

        [JsonIgnore]
        public virtual PurchaseObjectReady PurchaseObjectReady { get; set; }
    }
}
