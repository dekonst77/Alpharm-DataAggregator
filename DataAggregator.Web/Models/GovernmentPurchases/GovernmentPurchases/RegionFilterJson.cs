using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class RegionFilterJson
    {
        public List<string> FederalDistrict { get; set; }
        public List<string> FederationSubject { get; set; }
        public List<string> District { get; set; }
        public List<string> City { get; set; }
    }
}