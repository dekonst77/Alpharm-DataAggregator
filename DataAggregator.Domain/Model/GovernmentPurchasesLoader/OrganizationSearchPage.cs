using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

     [Table("OrganizationSearchPage", Schema = "org")]
    public class OrganizationSearchPage
    {
        public OrganizationSearchPage()
        {
            this.OrganizationLink = new HashSet<OrganizationLink>();
        }

        public long Id { get; set; }
        public long OrganizationSearchTaskId { get; set; }
        public int PageNumber { get; set; }
        public string Html { get; set; }
        public System.DateTime DateAdd { get; set; }
        public bool IsParsed { get; set; }

        public virtual ICollection<OrganizationLink> OrganizationLink { get; set; }
        public virtual OrganizationSearchTask OrganizationSearchTask { get; set; } 
    }
}