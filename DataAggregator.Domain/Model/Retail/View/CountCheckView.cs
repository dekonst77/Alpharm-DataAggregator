using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class CountCheckView
    {
        [Key]
        public long Id { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public string TradeName { get; set; }
        public string FormProduct { get; set; }
        public string OwnerTradeMark { get; set; }

   
        public decimal AvgSellingCount { get; set; }
        public decimal AvgSellingCountHalfYear { get; set; }
        public decimal Coefficient { get; set; }
    }
}
