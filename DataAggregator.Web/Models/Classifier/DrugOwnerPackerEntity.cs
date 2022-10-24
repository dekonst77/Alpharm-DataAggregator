using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Classifier
{
    public class DrugOwnerPackerEntity
    {
        public long DrugId { get; set; }
        public long PackerId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long OldClassifierId { get; set; }
    }
}