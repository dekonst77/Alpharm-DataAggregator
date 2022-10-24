using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.purchasefile
{
    [Table("FileAnalyzeTask", Schema = "purchasefile")]
    public class FileAnalyzeTask
    {
        public long Id { get; set; }
        public string PurchaseNumber { get; set; }
        public int StatusId { get; set; }
        public string FilePath { get; set; }
        public long FileLoadTaskId { get; set; }
        public string ErrorMessage { get; set; }

        public virtual FileLoadTask FileLoadTask { get; set; }
    }
}
