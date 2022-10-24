using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class RegionParametersUnderCommonView
    {
        public long Id { get; set; }

        public string RegionFullName { get; set; }

        public string Code { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int? TargetPharmacyCount { get; set; }

        public int? SourcePharmacyCount { get; set; }

        public int? Coefficient { get; set; }

        public int? CoefficientEditable { get; set; }

        public decimal? PurchaseSumNDS { get; set; }

        public decimal? PurchaseSumNDSCalculated { get; set; }

        public decimal? SellingSumNDS { get; set; }

        public decimal? SellingSumNDSCalculated { get; set; }

        public decimal? KnownSellingSumNDS { get; set; }

        public int? CoefficientSellingSumNDS { get; set; }

        public int NeedCheck { get; set; }

        public int UniteUnderCommonTerritory { get; set; }
    }
}
