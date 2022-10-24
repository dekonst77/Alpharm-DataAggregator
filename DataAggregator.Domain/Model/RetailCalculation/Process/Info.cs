using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.RetailCalculation
{
    [Table("Info", Schema = "process")]
    public class Info
    {
        [StringLength(250)]
        [Key]
        public string Param { get; set; }

        public string Value { get; set; }
    }
}
