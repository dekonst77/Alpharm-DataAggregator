using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Retail.FilterInfo
{
    public class SelectFilter
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Dictionary Source { get; set; }
    }
}