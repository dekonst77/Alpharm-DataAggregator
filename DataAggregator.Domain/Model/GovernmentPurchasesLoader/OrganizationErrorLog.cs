using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("OrganizationErrorLog", Schema = "org")]
    public class OrganizationErrorLog
    {
        public long Id { get; set; }
        public System.DateTime Date { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}