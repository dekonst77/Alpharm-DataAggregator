using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class LotFunding
    {
        public long Id { get; set; }

        public long LotId { get; set; }

        public Byte FundingId { get; set; }

        public virtual Lot Lot { get; set; }

        public virtual Funding Funding { get; set; }
    }
}
