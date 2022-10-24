using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class ContractObject
    {
        public long Id {get; set;}
        public string Name {get; set;}
        public string OKPD {get; set;}
        public string Unit {get; set;}
        public decimal? Amount {get; set;}
        public decimal? Price {get; set;}
        public decimal? Sum {get; set;}
        public long? ContractId {get; set;}
        // раскомментировать!!!
        //public DateTime Date { get; set; }

        public Contract Contract { get; set; }
    }
}
