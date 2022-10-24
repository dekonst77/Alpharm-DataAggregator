using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class DrugIdWithMinMaxPriceView
    {
        [Key, Column(Order = 1)]
        public string Source { get; set; }
        [Key, Column(Order = 2)]
        public int DrugId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? Coeff { get; set; }
    }
}
