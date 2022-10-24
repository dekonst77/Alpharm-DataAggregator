using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class Pharmacy
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long? NetId { get; set; }

        //public virtual IList<RawData> RawData { get; set; }

        public virtual Net Net { get; set; }
    }
}
