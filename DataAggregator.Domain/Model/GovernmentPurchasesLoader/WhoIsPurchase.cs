using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
     [Table("WhoIsPurchase", Schema = "Purchase")]
    public class WhoIsPurchase
    {
        public long Id { get; set; }
        public string WhoIsPurchasing { get; set; }
        public int IsAnalyze { get; set; }
        public string ErrorMessage { get; set; }
        public long? PageId { get; set; }
    }
}