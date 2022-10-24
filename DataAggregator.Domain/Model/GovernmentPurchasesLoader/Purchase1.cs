using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Purchase", Schema = "Purchase")]
    public class PurchaseFind
    {
        public int PageId { get; set; }
        public int? PrintPageId { get; set; }

        public int? PageObjectId { get; set; }
        public long Id { get; set; }
        public int? IsAnalyze { get; set; }
        public string ErrorMessage { get; set; }
        public System.DateTime? Date { get; set; }
        public int FileStatusId { get; set; }
  
    }
}