using System.Collections.Generic;
using Newtonsoft.Json;
using System;
namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Category
    {
        public Byte Id { get; set; }

        public string Name { get; set; }
        
        [JsonIgnore]
        public virtual IList<Nature> Nature { get; set; }

        [JsonIgnore]
        public virtual IList<Purchase> Purchase { get; set; } 
    }
}
