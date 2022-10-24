using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class PharmacyParametersView
    {
        public long Id { get; set; }

        public string Region { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public long? TargetPharmacyId { get; set; }

        public string TargetPharmacyNetName { get; set; }

        public long? SourcePharmacyId { get; set; }

        public string SourcePharmacyName { get; set; }

        public string SourcePharmacyEntityName { get; set; }

        public decimal? PurchaseSumNDS { get; set; }

        public decimal? SellingSumNDS { get; set; }

        public string PharmacyType { get; set; }

        public bool IsBenefit { get; set; }

        public int? CoefficientEditable { get; set; }

        public string RegionFullName { get; set; }

        public int? RegionTargetPharmacyCount { get; set; }

        public int? RegionSourcePharmacyCount { get; set; }

        public int? RegionCoefficient { get; set; }

        public int? RegionCoefficientEditable { get; set; }

        public decimal? SellingSumNDSCalculated { get; set; }
    }
}
