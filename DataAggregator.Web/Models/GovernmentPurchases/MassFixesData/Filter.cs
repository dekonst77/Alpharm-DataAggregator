using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.MassFixesData
{
    public class Filter
    {
        public string PurchaseNumber { get; set; }
        public string ReceiverFederationSubject { get; set; }
        public DateTime? DateBeginStart { get; set; }
        public DateTime? DateBeginEnd { get; set; }
        public DateTime? DateEndStart { get; set; }
        public DateTime? DateEndEnd { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverINN { get; set; }
        public string CategoryName { get; set; }
        public long? ReceiverId { get; set; }
        public string NatureName { get; set; }
        public string FundingName { get; set; }
    }
}