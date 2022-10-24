using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.AveragePrice
{
    public class SelectedDrugJson
    {
        public string Drug { get; set; }
        public long? ClassifierId { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
    }
}