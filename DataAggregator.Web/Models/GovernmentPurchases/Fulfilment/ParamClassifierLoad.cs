using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.Fulfilment
{
    public class ParamClassifierLoad
    {
        public long? ClassifierId { get; set; }
        public string INNGroup { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string Corporation { get; set; }
        public string Packer { get; set; }
    }
}