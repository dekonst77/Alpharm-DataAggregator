using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAggregator.Domain.Model.Retail.ExternalInteraction
{
    [Table("RegionCode", Schema = "External")]
    public class RegionCode
    {
        [Key]
        public string Code { get; set; }
    }
}
