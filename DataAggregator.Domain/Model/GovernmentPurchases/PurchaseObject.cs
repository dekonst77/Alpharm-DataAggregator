using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
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

        [JsonIgnore]
        public virtual Lot Lot { get; set; }
    }
}
