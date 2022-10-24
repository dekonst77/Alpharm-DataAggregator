using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.RetailCalculation
{
    [Table("Status", Schema = "process")]
    public class Status
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
