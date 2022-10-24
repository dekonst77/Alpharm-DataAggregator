using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.FileInfoService
{

    [Table("ErrorLog", Schema = "FileInfoService")]
    public class FileInfoServiceErrorLog
    {
        public long Id { get; set; }

        public string Date { get; set; }

        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }
    }
}