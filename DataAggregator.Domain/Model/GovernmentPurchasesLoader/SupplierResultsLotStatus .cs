using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("LotStatus", Schema = "dict")]
    public class SupplierResultsLotStatus 
    {
         public long Id { get; set; }
         public long LotStatusId { get; set; }
         public string Name { get; set; }
    }
}