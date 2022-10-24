using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Classifier
{
    public class DataTransferClassifierFilter
    {
        public long? DrugId { get; set; }
        public long? PackerId { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string TradeName { get; set; }
        public string Packer { get; set; }
        public string OwnerTradeMark { get; set; }
    }
}