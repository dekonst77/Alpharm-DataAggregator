using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.AveragePrice
{
    public class SelectedRegionJson
    {
        public string FederalDistrict { get; set; }
        public long? FederalDistrictId { get; set; }
        public string FederationSubject { get; set; }
        public long? FederationSubjectId { get; set; }
        public string District { get; set; }
        public long? DistrictId { get; set; }
        public string City { get; set; }
        public long? CityId { get; set; }
        public long? SelectedRegionId { get; set; }
    }
}