using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class AveragePriceView
    {
        public long Id { get; set; }
        public long RegionId { get; set; }
        public string FederalDistrict { get; set; }
        public string FederationSubject { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long ClassifierId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public decimal Price { get; set; }
    }
}
