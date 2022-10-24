using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("OKPD2", Schema = "search")]
    public class OKPD2
    {
        public long Id { get; set; }
        public string Name { get; set; } 
    }
}