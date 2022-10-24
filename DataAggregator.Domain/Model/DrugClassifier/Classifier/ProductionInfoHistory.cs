using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class ProductionInfoHistory
    {
        public long Id { get; set; }

        public long OwnerTradeMarkId { get; set; }

        public long PackerId { get; set; }

        public long DrugId { get; set; }

        public long ProductionInfoId { get; set; }

        public virtual ProductionInfo ProductionInfo { get; set; }
    }
}
