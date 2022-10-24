using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Classifier
{
    public class HistoryFilter
    {
        public int ClassifierId { get; set; }
        public int DrugId { get; set; }
        public int OwnerTradeMarkId { get; set; }
        public int PackerId { get; set; }
    }
}