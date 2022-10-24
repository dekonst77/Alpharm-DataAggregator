using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.purchasefile
{

    [Table("FileLoadTask", Schema = "purchasefile")]
    public class FileLoadTask
    {
        public long Id { get; set; }
        public string PurchaseNumber { get; set; }
        public int StatusId { get; set; }
        public string FileUrl { get; set; }
        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
    }


}
