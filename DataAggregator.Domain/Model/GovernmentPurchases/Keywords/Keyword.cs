using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Keywords
{
    [Table("Keyword", Schema = "Keywords")]
    public class Keyword
    {
        public long Id { get; set; }

        public string Text { get; set; }
    }
}
