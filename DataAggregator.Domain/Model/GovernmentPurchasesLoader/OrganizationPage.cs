using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
     [Table("OrganizationPage", Schema = "org")]
    public class OrganizationPage
    {
        public long Id { get; set; }
        public string Link { get; set; }
        public string Html { get; set; }
        public System.DateTime? DateLoad { get; set; }
        public bool IsAnalyzed { get; set; }
        public int? AnalyzedStatus { get; set; }
        public bool Is223 { get; set; } 
    }
}