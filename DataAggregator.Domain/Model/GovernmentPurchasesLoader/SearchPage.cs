using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("SearchPage", Schema = "search")]
    public class SearchPage
    {
        public SearchPage()
        {
            this.PurchaseLink = new HashSet<PurchaseLink>();
        }

        public long Id { get; set; }
        public long? SearchTaskId { get; set; }
        public int PageNumber { get; set; }
        public string Html { get; set; }
        public System.DateTime DateAdd { get; set; }
        public bool IsParsed { get; set; }

        public virtual ICollection<PurchaseLink> PurchaseLink { get; set; }
        public virtual SearchTask SearchTask { get; set; } 
    }
}