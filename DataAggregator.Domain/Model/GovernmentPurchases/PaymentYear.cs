using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class PaymentYear
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public virtual IList<Payment> Payment { get; set; } 
    }
}
