using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("OrganizationSearchTask", Schema = "org")]
    public class OrganizationSearchTask
    {
        public OrganizationSearchTask()
        {
            this.OrganizationSearchPage = new HashSet<OrganizationSearchPage>();
        }

        public long Id { get; set; }
        public string Law { get; set; }
        public string SearchString { get; set; }
        public int NextPage { get; set; }
        public bool IsLoaded { get; set; }
        public System.DateTime DateAdd { get; set; }
        public System.DateTime? LastTryLoad { get; set; }

        public virtual ICollection<OrganizationSearchPage> OrganizationSearchPage { get; set; } 
    }
}