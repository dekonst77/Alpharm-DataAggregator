﻿using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class RegionPM12View
    {
        [Key]
        public string RegionPM12 { get; set; }

        public string FullName { get; set; }
    }
}
