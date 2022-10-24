using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Funding
    {
        public Byte Id { get; set; }

        public string InternalName { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool CanGetTransfer { get; set; }

        public bool IsBudget { get; set; }

        public bool IsNotBudget { get; set; }

        public bool IsTransfer { get; set; }
        [JsonIgnore]
        public virtual IList<LotFunding> LotFunding { get; set; } 
    }
}
