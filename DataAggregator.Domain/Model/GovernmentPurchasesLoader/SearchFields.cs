using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("SearchFields", Schema = "org")]
    public class SearchFields
    {
        public string Header { get; set; }
        public string Key { get; set; }
        public string url { get; set; }
        public long? Count { get; set; }
        public bool? Is223 { get; set; }
        public long Id { get; set; } 
    }
}