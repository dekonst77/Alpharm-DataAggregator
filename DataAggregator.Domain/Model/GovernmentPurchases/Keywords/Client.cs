using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.Keywords
{
    [Table("Client", Schema = "Keywords")]
    public class Client
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
