using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Search
{
    [Table("ListType", Schema = "Search")]
    public class ListType
    {
        public long Id { get; set; }

        public string Value { get; set; }
    }
}
