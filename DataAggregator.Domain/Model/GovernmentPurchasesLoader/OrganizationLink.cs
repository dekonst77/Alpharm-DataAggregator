using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("OrganizationLink", Schema = "org")]
    public class OrganizationLink
    {
        public long Id { get; set; }
        public long OrganizationSearchPageId { get; set; }
        public string Number { get; set; }
        public string Link { get; set; }

        public virtual OrganizationSearchPage OrganizationSearchPage { get; set; } 
    }
}