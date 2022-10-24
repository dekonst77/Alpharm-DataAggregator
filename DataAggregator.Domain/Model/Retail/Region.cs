using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.Retail
{
    public class Region
    {
        [Key]
        public string Code { get; set; }
        public Nullable<int> Level { get; set; }
        public string ParentCode { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public Nullable<long> FederalDistrictId { get; set; }

        public virtual FederalDistrict FederalDistrict { get; set; }
    }
}