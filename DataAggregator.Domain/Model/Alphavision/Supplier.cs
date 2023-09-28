using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Alphavision
{
    [Table("Supplier", Schema = "dbo")]
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdAlphaVision { get; set; }
        public bool IsActual { get; set; }
    }
}