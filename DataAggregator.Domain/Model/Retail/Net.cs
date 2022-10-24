using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class Net
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public virtual IList<Pharmacy> Pharmacy { get; set; } 
    }
}
