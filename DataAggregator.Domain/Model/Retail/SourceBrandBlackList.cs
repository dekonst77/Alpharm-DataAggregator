using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("SourceBrandBlackList")]
    public class SourceBrandBlackList
    {
        [Key]
        [Column(Order = 1)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Month { get; set; }
        [Key]
        [Column(Order = 3)]
        public long SourceId { get; set; }
        [Key]
        [Column(Order = 4)]
        public long BrandId { get; set; }
    }
}
