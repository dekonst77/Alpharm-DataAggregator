using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class DeliveryTimePeriod
    {
        public Byte Id { get; set; }

        public string Name { get; set; }
        [JsonIgnore]
        public virtual IList<DeliveryTimeInfo> DeliveryTimeInfo { get; set; } 
    }
}
