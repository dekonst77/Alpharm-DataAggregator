using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("StagePurchase", Schema = "search")]
    public class StagePurchase
    {
        [Key]
        public string Key { get; set; }
        public string Name { get; set; } 
    }
}