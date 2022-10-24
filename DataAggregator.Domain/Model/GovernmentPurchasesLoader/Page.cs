using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Page", Schema = "Purchase")]
    public class Page
    {
        public Page()
        {
            this.PageTableHeader = new HashSet<PageTableHeader>();
            this.PageItem = new HashSet<PageItem>();
            this.PageTable = new HashSet<PageTable>();
        }

        public long Id { get; set; }
        public long Type { get; set; }
        public string Url { get; set; }
        public string PurchaseNumber { get; set; }
        public string Html { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime DateLoad { get; set; }
        public int IsAnalyze { get; set; }
        public string ErrorMessage { get; set; }
        public string ReestrNumber { get; set; }
        public bool NeedReload { get; set; }
        public System.DateTime? DateUpdate { get; set; }

        public virtual ICollection<PageTableHeader> PageTableHeader { get; set; }
        public virtual ICollection<PageItem> PageItem { get; set; }
        public virtual ICollection<PageTable> PageTable { get; set; }
        public DateTime? LastCheck { get; set; }
    }

}