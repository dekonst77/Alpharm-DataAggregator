using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class RegionPM01View
    {
        [Key]
        public string RegionPM01 { get; set; }

        public string FullName { get; set; }
    }
}
