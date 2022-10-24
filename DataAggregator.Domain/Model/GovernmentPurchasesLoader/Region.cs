using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Region", Schema = "org")]
    public class Region
    {
        public long Id { get; set; }
        public string FederalDistrict { get; set; }
        public string FederationSubject { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Code { get; set; } 
    }
}