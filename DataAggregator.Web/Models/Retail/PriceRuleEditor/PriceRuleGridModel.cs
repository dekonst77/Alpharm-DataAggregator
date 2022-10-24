using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Retail.PriceRuleEditor
{
    public class PriceRuleGridModel
    {
        public int ClassifierId { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
    }

}