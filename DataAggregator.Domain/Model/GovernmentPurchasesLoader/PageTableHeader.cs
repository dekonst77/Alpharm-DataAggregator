using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("PageTableHeader", Schema = "Purchase")]
    public class PageTableHeader
    {
        public long Id { get; set; }
        public string Header { get; set; }
        public long PageId { get; set; }
        public System.Guid PageTableGroupId { get; set; }

        public virtual Page Page { get; set; } 
    }
}