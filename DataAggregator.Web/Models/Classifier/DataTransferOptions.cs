using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Classifier
{
    public class DataTransferOptions
    {
        public bool TransferDrugId { get; set; }
        public bool TransferPackerId { get; set; }
        public bool TransferOwnerTradeMarkId { get; set; }
        public int? YearFrom { get; set; }
        public int? MonthFrom { get; set; }
        public int? YearTo { get; set; }
        public int? MonthTo { get; set; }
    }
}