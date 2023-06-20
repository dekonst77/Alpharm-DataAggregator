using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("SourceBrandBlackList")]
    public class SourceBrandBlackList
    {
        [Key]
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long SourceId { get; set; }
        public long BrandId { get; set; }
    }
}
