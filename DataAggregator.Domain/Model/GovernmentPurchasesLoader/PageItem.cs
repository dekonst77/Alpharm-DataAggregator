using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("PageItem", Schema = "Purchase")]
    public class PageItem
    {
        public long Id { get; set; }
        public long PageId { get; set; }
        public string Header { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Url { get; set; }

        public virtual Page Page { get; set; } 
    }
}