using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.QueryModel
{
    public class HasDataForAvgPrice
    {
        [Key, Column(Order = 1)]
        public bool HasSellingData { get; set; }

        [Key, Column(Order = 2)]
        public bool HasPurchaseData { get; set; }
    }
}
