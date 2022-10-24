using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Stage
    {
        public Byte Id { get; set; }

        public string Name { get; set; }

        public virtual IList<Purchase> Purchase { get; set; } 
    }
}
