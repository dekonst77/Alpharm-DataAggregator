using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("ErrorLog", Schema = "search")]
    public class ErrorLog
    {
        public long Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public System.DateTime Date { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
     
    }
}