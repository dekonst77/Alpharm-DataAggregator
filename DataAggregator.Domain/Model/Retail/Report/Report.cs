using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.Report
{
    [Table("Reports", Schema = "report")]
    public class Report
    {
        public long Id { get; set; }
        public string Name { get; set; }       
    }
}
