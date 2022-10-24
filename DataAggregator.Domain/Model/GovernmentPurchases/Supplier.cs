using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Supplier
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public string LocationAddress { get; set; }

        public string ContactMail { get; set; }

        public string PhoneNumber { get; set; }
    }
}
