using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    public class Date
    {
        [Key]
        public System.DateTime Date1 { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
    }
}