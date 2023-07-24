using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("TargetPharmacyOFDBlackList")]
    public class TargetPharmacyOFDBlackList
    {
        [Key]
        public int Id { get; set; }
        public long TargetPharmacyId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
