using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Nature
    {
        public Byte Id { get; set; }

        public Byte CategoryId { get; set; }

        public string Name { get; set; }
        public string NameMini { get; set; }

        public virtual Category Category { get; set; }

        [JsonIgnore]
        public virtual IList<Purchase> Purchase { get; set; } 
    }
    public class Nature_L2
    {
        public Int16 Id { get; set; }

        public Byte Nature_L1Id { get; set; }

        public string Name { get; set; }
        public virtual Nature Nature_L1 { get; set; }

        [JsonIgnore]
        public virtual IList<Purchase> Purchase { get; set; }
    }

}
