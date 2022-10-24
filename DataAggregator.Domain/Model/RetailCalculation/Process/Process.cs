using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.RetailCalculation
{
    [Table("Process", Schema = "process")]
    public class Process
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Procedure { get; set; }
    }
}
