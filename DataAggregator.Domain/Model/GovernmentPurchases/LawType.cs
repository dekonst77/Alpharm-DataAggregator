using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class LawType
    {
        public Byte Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public virtual IList<Purchase> Purchase { get; set; } 
    }
}
