using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("KOSGUAnalyzeTest", Schema = "temp")]
    public class KOSGUAnalyzeTest
    {
        public long PrintPageId { get; set; }
        public long Id { get; set; }
        public int? IsAnalyze { get; set; }
        public string ErrorMessage { get; set; }
        public System.DateTime? Date { get; set; }
      

    }
}