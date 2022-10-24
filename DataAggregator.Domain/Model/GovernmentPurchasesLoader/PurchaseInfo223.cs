using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("PurchaseInfo223", Schema = "Purchase")]
    public class PurchaseInfo223
    {
       
     
            public long Id { get; set; }
            public int PageId { get; set; }
            public int PageLotId { get; set; }
            public int? IsAnalyze { get; set; }
            public string ErrorMessage { get; set; }
            public System.DateTime? Date { get; set; }
            public string PurchaseNumber { get; set; }
            public string Url { get; set; }
        
    }
}