using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.RetailCalculation
{
    [Table("TargetPharmacyWithoutAverage", Schema = "In")]
    public class TargetPharmacyWithoutAverageIn
    {
        [Key]
        [Column(Order = 1)]
        public int TargetPharmacyId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 3)]
        public int Month { get; set; }
        public bool IsGenerated { get; set; }
    }

    [Table("TargetPharmacyWithoutAverage", Schema = "Out")]
    public class TargetPharmacyWithoutAverageOut
    {
        [Key]
        [Column(Order = 1)]
        public int TargetPharmacyId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 4)]
        public int Month { get; set; }
        public bool IsGenerated { get; set; }
    }
}
