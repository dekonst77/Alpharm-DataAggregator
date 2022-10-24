using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class PurchaseInWork
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }
        public string URL { get; set; }

        public Guid UserId { get; set; }
    }
}
