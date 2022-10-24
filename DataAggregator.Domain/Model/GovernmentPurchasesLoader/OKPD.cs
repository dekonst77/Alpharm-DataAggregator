using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("OKPD", Schema = "search")]
    public class OKPD
    {
        public long Id { get; set; }
        public string Name { get; set; } 
    }
}